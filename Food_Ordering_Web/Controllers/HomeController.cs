using Core.ViewModels;
using Food_Ordering_Web.Models;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;


namespace Food_Ordering_Web.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public async Task<IActionResult> Index()
        {
            // Prepare the request URL. For example, let's say your API is running locally on port 5001
            string requestUrl = "https://localhost:7248/api/auth/isUserSignedIn";

            // Execute the request using HttpClient
            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

            // Check the response
            if (response.IsSuccessStatusCode)
            {
                // Read the content as a string
                string contentString = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON string into a C# object
                var userStatus = JsonSerializer.Deserialize<UserStatusDTO>(contentString);

                if (userStatus.IsSignedIn)
                {
                    // Get user's role and redirect accordingly
                    var userRole = userStatus.Roles.FirstOrDefault(); // Assuming there's at least one role

                    // Redirect based on role
                    return RedirectToRoleBasedView(userRole, "/");
                }
            }
            else
            {
                // Log or handle errors here
                _logger.LogError($"Failed to check user status. API responded with status code {response.StatusCode}");
            }

            return View();
        }

        
        public async Task<IActionResult> Login(LoginModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("API_URL_HERE/Auth/Login", model);

            if (response.IsSuccessStatusCode)
            {
                var authInfo = await response.Content.ReadFromJsonAsync<AuthResponse>();
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = authInfo.Expiration
                };
                Response.Cookies.Append("JWTToken", authInfo.Token, cookieOptions);
            }

            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        public class AuthResponse
        {
            public string Token { get; set; }
            public DateTime Expiration { get; set; }
        }

        private IActionResult RedirectToRoleBasedView(string userRole, string defaultUrl)
        {
            switch (userRole)
            {
                case "Admin":
                    return RedirectToAction("Index", "Admin");
                case "Customer":
                    return RedirectToAction("Index", "Customer");
                case "Restaurant":
                    return RedirectToAction("Index", "Restaurant");
                default:
                    return LocalRedirect(defaultUrl ?? "/");  // If defaultUrl is null, redirect to home page
            }
        }
        [Authorize]
        public IActionResult Profile()
        {
            return View("~/Views/Home/Profile.cshtml");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        // Define a DTO to map API response
        public class UserStatusDTO
        {
            public bool IsSignedIn { get; set; }
            public List<string> Roles { get; set; }
        }
    }
}