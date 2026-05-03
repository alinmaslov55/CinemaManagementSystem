using CinemaSystem.DataAccess.Data;
using CinemaSystem.DataAccess.DbInitializer.Seeders;
using CinemaSystem.Models.Entities;
using CinemaSystem.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CinemaSystem.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        private readonly IMovieSyncService _syncService;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment env,
            IMovieSyncService syncService,
            ILogger<DbInitializer> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
            _syncService = syncService;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Începe aplicarea migrărilor...");
                if (_context.Database.GetPendingMigrations().Any())
                {
                    await _context.Database.MigrateAsync();
                }

                _logger.LogInformation("Începe popularea bazei de date (Seeding)...");
                var basePath = _env.WebRootPath;

                await IdentitySeeder.SeedAsync(_userManager, _roleManager);

                await CinemaSeeder.SeedAsync(_context, basePath);

                await MovieSeeder.SeedAsync(_context, basePath, _syncService);

                await ShowtimeSeeder.SeedAsync(_context);

                _logger.LogInformation("Popularea bazei de date s-a finalizat cu succes.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eroare critică în procesul de inițializare a bazei de date.");
                throw;
            }
        }
    }
}