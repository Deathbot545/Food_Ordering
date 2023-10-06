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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Google;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Net;
using Azure;

namespace Food_Ordering_API.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpClient = new HttpClient(); // Or however you get your HttpClient
            _apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl");  // Initialize it here
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }
            return View("~/Views/Account/Signup.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(string username, string password, string roleName)
        {
            string apiEndpoint = $"Register/{roleName}";

            return await AddUserToApi(apiEndpoint, username, password, "YourActionName", "YourControllerName",roleName);
        }


        private async Task<IActionResult> AddUserToApi(string apiEndpoint, string username, string password, string actionName, string controllerName, string roleName) // added roleName parameter
        {
            var user = new { Username = username, Password = password };
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/{apiEndpoint}",
                new StringContent(System.Text.Json.JsonSerializer.Serialize(user), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(responseContent);

                if (responseObject != null && responseObject.Token != null)
                {
                    HttpContext.Response.Cookies.Append("jwtCookie", responseObject.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });

                    return await HandleLogin(username, responseObject.Token, responseObject.UserId);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Token was not provided.");
                    return Content("An error occurred.");
                }
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                var errorObject = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(error);

                if (errorObject != null && errorObject.ContainsKey("message"))
                {
                    TempData["ErrorMessage"] = errorObject["message"];
                }

                return RedirectToAction("Register");
                // Redirect to the specified action and controller
            }
        }



        public IActionResult Login()
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }
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

            var jsonPayload = System.Text.Json.JsonSerializer.Serialize(loginDto);
            var httpResponse = await _httpClient.PostAsync(
                $"{_apiBaseUrl}/Login",
                new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var responseObject = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(responseContent);

                if (responseObject != null)
                {
                    HttpContext.Response.Cookies.Append("jwtCookie", responseObject.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });

                    return await HandleLogin(model.UserName, responseObject.Token, responseObject.UserId);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Could not deserialize response.");
                    ViewBag.ErrorMessage = "Could not deserialize response.";
                }
            }
            else
            {
                // Parse the error message from API
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var errorResponse = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);

                if (errorResponse != null && errorResponse.ContainsKey("message"))
                {
                    ViewBag.ErrorMessage = errorResponse["message"];
                }
                else
                {
                    ViewBag.ErrorMessage = "An unknown error occurred.";
                }
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
      

        [HttpGet]
        public IActionResult ExternalRegisterOrLogin(string role = null)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", new { role = role })
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse(string role = null)
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (result?.Succeeded == true)
            {
                // User is authenticated
                _logger.LogInformation("User authenticated.");
            }
            else
            {
                // User is not authenticated
                _logger.LogError($"Authentication failed. Reason: {result?.Failure?.Message}");
                return Unauthorized(); // or some other action to handle failure
            }


            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            if (claims == null)
            {
                return BadRequest("No claims found.");
            }

            string email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email claim missing.");
            }

            if (!string.IsNullOrEmpty(role))
            {
                // Your existing UserDto creation
                UserDto userDto = new UserDto { Username = email, Password = "" };

                var json = JsonConvert.SerializeObject(userDto);
                _logger.LogInformation($"JSON Payload: {json}");
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var fullUrl = $"{_apiBaseUrl}/Register/{role}";
                _logger.LogInformation($"Full URL: {fullUrl}");

                var apiResponse = await _httpClient.PostAsync(fullUrl, content);

                if (apiResponse != null)
                {
                    var apiResponseContent = await apiResponse.Content.ReadAsStringAsync();
                    if (apiResponse.IsSuccessStatusCode)
                    {
                        var apiResponseObject = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(apiResponseContent);  // Assume RegisterResponse has a Token field

                        if (apiResponseObject != null)
                        {
                            HttpContext.Response.Cookies.Append("jwtCookie", apiResponseObject.Token, new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict
                            });

                            return await HandleLogin(email, apiResponseObject.Token, apiResponseObject.UserId);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Could not deserialize response.");
                            return RedirectToAction("SuccessAction");
                        }
                    }
                    else if (apiResponse.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var error = await apiResponse.Content.ReadAsStringAsync();
                        var errorObject = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(error);

                        if (errorObject != null && errorObject.ContainsKey("message"))
                        {
                            TempData["ErrorMessage"] = errorObject["message"];
                        }

                        return RedirectToAction("Register");
                    }
                    else
                    {
                        _logger.LogError("API Response is unsuccessful");
                        return StatusCode((int)HttpStatusCode.InternalServerError);
                    }
                }
                else
                {
                    _logger.LogError("API Response is null");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }


            }
            else
            {
                // Login flow
                LoginDto loginDto = new LoginDto { UsernameOrEmail = email, Password = "" };
                var json = JsonConvert.SerializeObject(loginDto);
                _logger.LogInformation($"JSON Payload: {json}");
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var fullUrl = $"{_apiBaseUrl}/GoogleLogin";
                _logger.LogInformation($"Full URL: {fullUrl}");
                var response = await _httpClient.PostAsync(fullUrl, content);
                //...
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseObject = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(responseContent);

                    if (responseObject != null)
                    {
                        HttpContext.Response.Cookies.Append("jwtCookie", responseObject.Token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict
                        });

                        return await HandleLogin(email, responseObject.Token, responseObject.UserId);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Could not deserialize response.");
                    }
                    return RedirectToAction("SuccessAction");
                }
                //...

                else
                {
                    ModelState.AddModelError(string.Empty, "User not registered or invalid credentials.");
                    TempData["ErrorMessage"] = "User not registered or invalid credentials."; // Using TempData to store the error message
                    return RedirectToAction("Login", "Account"); // Redirecting to Login action in Account controller
                }
            }

        }
        public async Task<IActionResult> HandleLogin(string email, string token, string userId)
        {
            // Decode JWT to get role
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(token) as JwtSecurityToken;

            // Check for role claim safely
            var roleClaim = tokenS.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

            if (roleClaim != null)
            {
                var role = roleClaim.Value;

                // Retrieve the ApplicationUser object for the current user.
                var user = await _userManager.FindByNameAsync(email);

                if (user != null)
                {
                    // Create claims
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, role),
                new Claim("UserId", userId)
            };

                    // Create ClaimsIdentity
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Use SignInManager to sign in the user
                    await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, claimsIdentity.Claims);

                    // Redirect based on role
                    return RedirectToAction("Index", role); // Assuming you have an Index action for each role.
                }
                else
                {
                    return BadRequest("User not found.");
                }
            }
            else
            {
                return BadRequest("Role claim not found in JWT.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // HTTP POST to the API to invalidate the token or perform other logout activities
            var httpResponse = await _httpClient.PostAsync($"{_apiBaseUrl}/Logout", null);

            if (httpResponse.IsSuccessStatusCode)
            {
                // Remove JWT token from HttpOnly cookie
                Response.Cookies.Delete("jwtCookie");

                // Use SignInManager to sign out the user
                await _signInManager.SignOutAsync();

                // Redirect to another page (e.g., login page)
                return RedirectToAction("Login", "Account");
            }
            else
            {
                // If you wish, you can read the error message from API and display it
                var error = await httpResponse.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Failed to logout: {error}");

                // Return to the same or different view as needed
                return View();
            }
        }
    }
}
