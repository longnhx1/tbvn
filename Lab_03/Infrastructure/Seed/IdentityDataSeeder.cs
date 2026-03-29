using System.Linq;
using Lab_03.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lab_03.Infrastructure.Seed;

public static class IdentityDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration, ILogger logger)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = [SD.Role_Admin, SD.Role_Employee, SD.Role_Company, SD.Role_Customer];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var r = await roleManager.CreateAsync(new IdentityRole(role));
                if (!r.Succeeded)
                    logger.LogError("Không tạo được role {Role}: {Errors}", role, string.Join(", ", r.Errors.Select(e => e.Description)));
            }
        }

        var adminEmail = configuration["SeedAdmin:Email"] ?? "admin@travelbooking.local";
        var adminPassword = configuration["SeedAdmin:Password"] ?? "Admin@123456";
        var adminName = configuration["SeedAdmin:FullName"] ?? "Quản trị viên";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = adminName
            };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(adminUser, SD.Role_Admin);
            else
                logger.LogError("Không tạo được tài khoản admin {Email}: {Errors}", adminEmail, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        else if (!await userManager.IsInRoleAsync(adminUser, SD.Role_Admin))
        {
            await userManager.AddToRoleAsync(adminUser, SD.Role_Admin);
        }
    }
}
