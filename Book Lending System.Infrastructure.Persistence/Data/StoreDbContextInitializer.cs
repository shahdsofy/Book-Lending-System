using Book_Lending_System.Core.Contracts.Persistence;
using Book_Lending_System.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Book_Lending_System.Infrastructure.Persistence.Data
{
    public class StoreDbContextInitializer(StoreDbContext dbContext
        , RoleManager<IdentityRole> roleManager
        , UserManager<ApplicationUser> userManager) : IStoreDbContextInitializer
    {
        public async Task InitializeAsync()
        {
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                await dbContext.Database.MigrateAsync();
            }
        }

        public async Task SeedAsync()
        {

            if (!dbContext.Roles.Any())
            {
                var roles = new List<string> { "Admin", "Member" };

                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            if (!userManager.Users.Any(u => u.UserName == "Admin"))
            {
                var admin = new ApplicationUser()
                {
                    DisplayName = "Admin",
                    UserName = "Admin",
                    Email = "shahdsofy12@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumber = "1234567890",
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");

                // 3- Assign Admin Role
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
