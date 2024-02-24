using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NextNews.Data;
using NextNews.Models.Database;

namespace NextNews.Services
{
    public class RoleManagementService : IRoleManagementService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public RoleManagementService(RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }


        public async Task<bool> CreateRoleAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

                // Optionally, you can perform additional operations or logging based on the result.
                // Example: if (!result.Succeeded) { /* Handle failure */ }

                return result.Succeeded;
            }

            return false; // Role already exists
        }

    }
}
