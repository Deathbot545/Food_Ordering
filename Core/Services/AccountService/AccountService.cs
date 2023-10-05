using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.AccountService
{
    public class AccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;

        }

        public async Task AddAdminAsync(string username, string password)
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            var user = new ApplicationUser { UserName = username, Email = username };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {

               
                await _userManager.AddToRoleAsync(user, "Admin");

            }
            else
            {
                throw new IdentityException(result.Errors);
            }
        }

        public async Task AddCustomerAsync(string username, string password)
        {
            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
            }
            var user = new ApplicationUser { UserName = username, Email = username };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
              

                await _userManager.AddToRoleAsync(user, "Customer");
            }
            else
            {
                throw new IdentityException(result.Errors);
            }
        }

        public async Task AddRestaurantAsync(string username, string password)
        {
            if (!await _roleManager.RoleExistsAsync("Restaurant"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Restaurant"));
            }
            var user = new ApplicationUser { UserName = username, Email = username };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
               
                await _userManager.AddToRoleAsync(user, "Restaurant");
            }
            else
            {
                throw new IdentityException(result.Errors);
            }
        }

        public async Task<ApplicationUser> LoginAsync(string identifier, string password)
        {
            var user = await _userManager.FindByEmailAsync(identifier) ?? await _userManager.FindByNameAsync(identifier);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
            {
                throw new InvalidLoginException();
            }
            return user;
        }


        // Custom exceptions to better convey specific error scenarios
        public class IdentityException : Exception
        {
            public IEnumerable<IdentityError> Errors { get; }

            public IdentityException(IEnumerable<IdentityError> errors)
            {
                Errors = errors;
            }
        }

        public class InvalidLoginException : Exception { }

        public class UserNotFoundException : Exception { }
    }
}
