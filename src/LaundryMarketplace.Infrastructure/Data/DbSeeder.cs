using LaundryMarketplace.Core.Enums;
using LaundryMarketplace.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LaundryMarketplace.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUserIdentity>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created / migrated
            await context.Database.MigrateAsync();

            // Seed Roles
            string[] roles = { Roles.Customer, Roles.Laundromat, Roles.Driver, Roles.NpoAdmin, Roles.SystemAdmin };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed System Admin
            await CreateUserIfNotExists(userManager, "admin@dignity.sa", "System Administrator", "Admin@123!", Roles.SystemAdmin);

            // Seed NPO Admin
            await CreateUserIfNotExists(userManager, "npo@dignity.sa", "NPO Administrator", "Npo@123!", Roles.NpoAdmin);

            // Seed sample Customer
            await CreateUserIfNotExists(userManager, "customer@example.com", "Thabo Molefe", "Customer@123!", Roles.Customer);

            // Seed sample Laundromat owner
            var laundromatUser = await CreateUserIfNotExists(userManager, "laundromat@example.com", "CleanWave Laundry", "Laundry@123!", Roles.Laundromat);
            if (laundromatUser != null && !await context.Laundromats.AnyAsync())
            {
                context.Laundromats.Add(new Core.Entities.Laundromat
                {
                    UserId = laundromatUser.Id,
                    BusinessName = "CleanWave Laundry - Germiston",
                    Address = "12 Industrial Road, Germiston, Ekurhuleni",
                    MetroZone = "Ekurhuleni",
                    CommissionRate = 12.00m,
                    IsActive = true
                });
                await context.SaveChangesAsync();
            }
        }

        private static async Task<ApplicationUserIdentity?> CreateUserIfNotExists(
            UserManager<ApplicationUserIdentity> userManager,
            string email, string fullName, string password, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null) return user;

            user = new ApplicationUserIdentity
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
                return user;
            }
            return null;
        }
    }
}
