using CinemaSystem.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace CinemaSystem.DataAccess.DbInitializer.Seeders
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (await roleManager.RoleExistsAsync("Admin")) return;

            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("Customer"));

            var adminUser = new ApplicationUser
            {
                UserName = "admin@ethereal.com",
                Email = "admin@ethereal.com",
                FullName = "Ethereal Admin",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "Admin123!");
            await userManager.AddToRoleAsync(adminUser, "Admin");

            var customer = new ApplicationUser
            {
                UserName = "john@example.com",
                Email = "john@example.com",
                FullName = "John Doe",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(customer, "Customer123!");
            await userManager.AddToRoleAsync(customer, "Customer");
        }
    }
}