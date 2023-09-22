using Core.Services.AccountService;
using Food_Ordering_Web.Models;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Core.Services.AccountService.AccountService;

namespace Food_Ordering_API.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(AccountService accountService, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _accountService = accountService;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Account/Signup.cshtml");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Account/Login.cshtml");
        }
        public async Task<IActionResult> AddAdmin(string email, string password)
        {
            try
            {
                await _accountService.AddAdminAsync(email, password);
                return RedirectToAction("Index", "Admin");
            }
            catch (IdentityException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        public async Task<IActionResult> AddCustomer(string email, string password)
        {
            try
            {
                await _accountService.AddCustomerAsync(email, password);
                return RedirectToAction("Index", "Customer");
            }
            catch (IdentityException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        public async Task<IActionResult> AddRestaurant(string email, string password)
        {
            try
            {
                await _accountService.AddRestaurantAsync(email, password);
                return RedirectToAction("Index", "Restaurant");
            }
            catch (IdentityException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                var user = await _accountService.LoginAsync(model.UserName, model.Password); // or model.email if you kept it
                await _signInManager.SignInAsync(user, isPersistent: false);

                var roles = await _userManager.GetRolesAsync(user);
                string userRole = roles.FirstOrDefault();

                switch (userRole)
                {
                    case "Admin":
                        return RedirectToAction("Index", "Admin");
                    case "Customer":
                        return RedirectToAction("Index", "Customer");
                    case "Restaurant":
                        return RedirectToAction("Index", "Restaurant");
                    default:
                        return RedirectToAction("Index", "Home");
                }
            }
            catch (UserNotFoundException)
            {
                return BadRequest("User not found");
            }
            catch (InvalidLoginException)
            {
                return BadRequest("Invalid login credentials");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string role)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            properties.Items["role"] = role;
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                return View("Error", new ErrorViewModel { Message = $"Error from external provider: {remoteError}" });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    string userRole = roles.FirstOrDefault();
                    return RedirectToRoleBasedView(userRole, returnUrl);
                }
            }

            var role = info.AuthenticationProperties.Items["role"];

            // Check if the role is null or empty
            // Check if the role is null or empty
            if (string.IsNullOrEmpty(role))
            {
                ViewBag.ErrorMessage = "User is not registered.";
                return View("Login");
            }


            user = new ApplicationUser { UserName = email, Email = email };
            var identityResult = await _userManager.CreateAsync(user);
            if (identityResult.Succeeded)
            {
                identityResult = await _userManager.AddLoginAsync(user, info);
                if (identityResult.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    }

                    await _userManager.AddToRoleAsync(user, role);

                    var roles = await _userManager.GetRolesAsync(user);
                    string userRole = roles.FirstOrDefault();

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToRoleBasedView(userRole, returnUrl);
                }
            }

            return View("Error", new ErrorViewModel { Message = "An error occurred while processing your authentication." });
        }
        // New helper method for role-based redirection
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

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
