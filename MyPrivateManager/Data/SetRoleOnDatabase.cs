using Microsoft.AspNetCore.Identity;

namespace MyPrivateManager.Data;

public static class SetRoleOnDatabase
{
    public static async void CreateRoleOnDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider
                            .GetRequiredService<RoleManager<IdentityRole>>();
        await SetRoleAsync(roleManager);
    }
    static async Task SetRoleAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "admin", "user" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
