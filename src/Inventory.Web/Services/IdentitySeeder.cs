using Microsoft.AspNetCore.Identity;

namespace Inventory.Web.Services;

public static class IdentitySeeder
{
    public const string AdminRole = "Admin";
    public const string UserRole = "User";

    public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        foreach (var role in new[] { AdminRole, UserRole })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        async Task EnsureUser(string email, string password, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded) return;
            }
            if (!await userManager.IsInRoleAsync(user, role))
                await userManager.AddToRoleAsync(user, role);
        }

        var adminEmail = config["Seed:AdminEmail"] ?? "admin@inventory.local";
        var adminPassword = config["Seed:AdminPassword"] ?? "Admin#12345";
        var userEmail = config["Seed:UserEmail"] ?? "user@inventory.local";
        var userPassword = config["Seed:UserPassword"] ?? "User#12345";

        await EnsureUser(adminEmail, adminPassword, AdminRole);
        await EnsureUser(userEmail, userPassword, UserRole);
    }
}
