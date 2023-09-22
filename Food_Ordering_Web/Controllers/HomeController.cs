using Food_Ordering_Web.Models;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Food_Ordering_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            // Check if user is signed in
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    // Get user's role
                    var roles = await _userManager.GetRolesAsync(user);
                    var userRole = roles.FirstOrDefault();

                    // Redirect based on role
                    return RedirectToRoleBasedView(userRole, "/");
                }
            }
            return View();
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


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}