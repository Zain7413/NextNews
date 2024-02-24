using Microsoft.AspNetCore.Identity;

namespace NextNews.Models.Database
{
    public class SeedData
    {
        
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<SeedData> _logger;

       

        public SeedData(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, ILogger<SeedData> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }


       

        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var logger = serviceProvider.GetRequiredService<ILogger<SeedData>>();

            var seedData = new SeedData(roleManager, userManager, logger);
            await seedData.SeedRoles();
            await seedData.SeedUsers();
        }


       

        private async Task SeedRoles()
        {
            string[] roleNames = { "Admin", "Editor" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                    _logger.LogInformation($"Role '{roleName}' created.");
                }
                else
                {
                    _logger.LogInformation($"Role '{roleName}' already exists.");
                }
            }
        }

      

        private async Task SeedUsers()
        {
            await SeedUser("admin@example.com", "Admin", "User", "admin@example.com", new DateTime(1990, 1, 1), "Admin123@", "Admin");
            await SeedUser("editor@example.com", "Editor", "User", "editor@example.com", new DateTime(1995, 1, 1), "Editor123@", "Editor");
        }

        private async Task SeedUser(string userName, string firstName, string lastName, string email, DateTime dateOfBirth, string password, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                var seedUser = new User
                {
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                DateofBirth = dateOfBirth,
                
            };

                var result = await _userManager.CreateAsync(seedUser, password);
          
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(seedUser, roleName);
                    _logger.LogInformation($"User {seedUser.UserName} created successfully.");
                }
                else
                {
                    _logger.LogError($"User creation failed. Errors: {string.Join(", ", result.Errors)}");
                }
            }
        }







    }
}
