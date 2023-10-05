using Food_Ordering_Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities;
using Core.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Models;

namespace Food_Ordering_API.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpClient = new HttpClient(); // Or however you get your HttpClient
            _apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl");  // Initialize it here
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Account/Signup.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> AddAdmin(string username, string password)
        {
            return await AddUser("RegisterAdmin", username, password, "Index", "Admin");
        }
        [HttpPost]
        public async Task<IActionResult> AddCustomer(string username, string password)
        {
            return await AddUser("RegisterCustomer", username, password, "Index", "Customer");
        }
        [HttpPost]
        public async Task<IActionResult> AddRestaurant(string username, string password)
        {
            return await AddUser("RegisterRestaurant", username, password, "Index", "Restaurant");
        }
        [HttpPost]
        private async Task<IActionResult> AddUser(string apiEndpoint, string username, string password, string actionName, string controllerName)
        {
            var user = new { Username = username, Password = password };


            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/{apiEndpoint}",
                new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(actionName, controllerName);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Server error: {error}");
                return Content("An error occurred.");  // Changed this line to return a simple content result.
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string actionName, string controllerName)
        {
            var loginDto = new LoginDto
            {
                UsernameOrEmail = model.UserName,
                Password = model.Password
            };

            var jsonPayload = JsonSerializer.Serialize(loginDto);
            var httpResponse = await _httpClient.PostAsync(
                $"{_apiBaseUrl}/Login",
                new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<LoginResponse>(responseContent);

                if (responseObject != null)
                {
                    // Store JWT in HttpOnly cookie
                    HttpContext.Response.Cookies.Append("jwtCookie", responseObject.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });

                    // Decode JWT to get role
                    var handler = new JwtSecurityTokenHandler();
                    var tokenS = handler.ReadToken(responseObject.Token) as JwtSecurityToken;

                    // Check for role claim safely
                    var roleClaim = tokenS.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

                    if (roleClaim != null)
                    {
                        var role = roleClaim.Value;

                        // Retrieve the ApplicationUser object for the current user.
                        var user = await _userManager.FindByNameAsync(model.UserName);

                        if (user != null)
                        {
                            // Create claims
                            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.UserName),
                        new Claim(ClaimTypes.Role, role),
                        new Claim("UserId", responseObject.UserId)
                    };

                            // Create ClaimsIdentity
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            // Use SignInManager to sign in the user
                            await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, claimsIdentity.Claims);

                            // Redirect based on role
                            if (role == "Admin")
                            {
                                return RedirectToAction("Index", "Admin");
                            }
                            else if (role == "Customer")
                            {
                                return RedirectToAction("Index", "Customer");
                            }
                            else if (role == "Restaurant")
                            {
                                return RedirectToAction("Index", "Restaurant");
                            }
                            // ... Handle other roles as necessary ...
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "User not found.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Role claim not found in JWT.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Could not deserialize response.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }


        public class LoginResponse
        {
            [JsonPropertyName("message")]
            public string Message { get; set; }

            [JsonPropertyName("userId")]
            public string UserId { get; set; }

            [JsonPropertyName("token")]
            public string Token { get; set; }
        }





        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var httpResponse = await _httpClient.PostAsync($"{_apiBaseUrl}/Logout", null);

            if (httpResponse.IsSuccessStatusCode)
            {
                // Remove the JWT token from client-side or server-side storage
                return RedirectToAction("SomeAction", "SomeController"); // Redirect to another page
            }

            ModelState.AddModelError(string.Empty, "Failed to logout.");
            return View(); // Return to the same or different view as needed
        }




    }
}
