using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AstraReconsSaas.Data;

namespace AstraReconsSaas.Services
{
    public class AuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ProtectedSessionStorage _sessionStorage;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ProtectedSessionStorage sessionStorage)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _sessionStorage = sessionStorage;
        }

        public async Task<bool> RegisterUserAsync(string email, string password, string firstName, string lastName, string companyName)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                CompanyName = companyName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                // Add default user role
                await _userManager.AddToRoleAsync(user, "User");
                return true;
            }

            return false;
        }

        public async Task<bool> LoginAsync(string email, string password, bool rememberMe)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
            return result.Succeeded;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            await _sessionStorage.DeleteAsync("authToken");
        }

        public async Task<ApplicationUser> GetCurrentUserAsync(ClaimsPrincipal principal)
        {
            return await _userManager.GetUserAsync(principal);
        }

        public async Task<bool> IsInRoleAsync(ClaimsPrincipal principal, string role)
        {
            var user = await _userManager.GetUserAsync(principal);
            if (user == null)
                return false;

            return await _userManager.IsInRoleAsync(user, role);
        }
    }
}
