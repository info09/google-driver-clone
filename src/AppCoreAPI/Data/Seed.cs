using AppCoreAPI.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppCoreAPI.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var roles = new List<AppRole> { new AppRole { DisplayName = "User", Name = "User" }, new AppRole { DisplayName = "Admin", Name = "Admin" } };

            if (!await roleManager.Roles.AnyAsync())
            {
                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }
            }

            var admin = new AppUser { UserName = "admin", DisplayName = "Admin" };

            await userManager.CreateAsync(admin, "Admin@123$");
            await userManager.AddToRolesAsync(admin, ["Admin", "User"]);
        }
    }
}
