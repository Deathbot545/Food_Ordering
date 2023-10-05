using Core.DTO;
using Core.Services.AccountService;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Core.Services.AccountService.AccountService;

namespace Food_Ordering_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountApiController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager; // <-- Add this line

        public AccountApiController( AccountService accountService,UserManager<ApplicationUser> userManager,  RoleManager<IdentityRole> roleManager,    IConfiguration configuration)
        {
            _accountService = accountService;
            _userManager = userManager;
            _roleManager = roleManager; // <-- Add this line
            _configuration = configuration;
        }

        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> AddAdmin([FromBody] UserDto model)
        {
            try
            {
                string roleName = "Admin";


                await _accountService.AddAdminAsync(model.Username, model.Password);
                var user = await _userManager.FindByNameAsync(model.Username);
                await _userManager.AddToRoleAsync(user, roleName);

                return Ok(new { Message = "Admin added successfully" });
            }
            catch (IdentityException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        [HttpPost("RegisterCustomer")]
        public async Task<IActionResult> AddCustomer([FromBody] UserDto model)
        {
            string roleName = "Customer";


            await _accountService.AddCustomerAsync(model.Username, model.Password);
            var user = await _userManager.FindByNameAsync(model.Username);
            await _userManager.AddToRoleAsync(user, roleName);

            return Ok(new { Message = "Customer added successfully" });
        }

        [HttpPost("RegisterRestaurant")]
        public async Task<IActionResult> AddRestaurant([FromBody] UserDto model)
        {
            string roleName = "Restaurant";

            await _accountService.AddRestaurantAsync(model.Username, model.Password);
            var user = await _userManager.FindByNameAsync(model.Username);
            await _userManager.AddToRoleAsync(user, roleName);

            return Ok(new { Message = "Restaurant added successfully" });
        }

        // Your API
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            try
            {
                var user = await _accountService.LoginAsync(model.UsernameOrEmail, model.Password);
                var roles = await _userManager.GetRolesAsync(user);

                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds
                );

                return Ok(new
                {
                    message = "Successfully logged in",
                    userId = user.Id,
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            catch (AccountService.UserNotFoundException)
            {
                return NotFound(new { message = "User not found" });
            }
            catch (AccountService.InvalidLoginException)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            // Remove the token from client storage via client-side code.
            // Optionally, add the token to a server-side blacklist.
            return Ok(new { Message = "Logged out successfully" });
        }


    }
}
