using CinemaSystem.DataAccess.Data;
using CinemaSystem.Models.Entities;
using CinemaSystem.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CinemaSystem.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }

        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                // log this error.
            }

            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();

                var adminUser = new ApplicationUser
                {
                    UserName = "admin@cinema.com",
                    Email = "admin@cinema.com",
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };

                var result = _userManager.CreateAsync(adminUser, "Admin123*").GetAwaiter().GetResult();

                if (result.Succeeded)
                {
                    var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@cinema.com");
                    _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
                }
            }
        }
    }
}