using CinemaSystem.DataAccess.Repository.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.Web.BackgroundServices
{
    public class SeatHoldCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SeatHoldCleanupService> _logger;

        // Notice we DO NOT inject IUnitOfWork here. We inject the Service Provider.
        public SeatHoldCleanupService(IServiceProvider serviceProvider, ILogger<SeatHoldCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Seat Hold Cleanup Service is starting.");

            // Modern .NET implementation for periodic background tasks (e.g., tick every 5 minutes)
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

            try
            {
                // This loop runs indefinitely until the application shuts down
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    CleanUpExpiredHolds();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Seat Hold Cleanup Service is stopping.");
            }
        }

        private void CleanUpExpiredHolds()
        {
            // CRITICAL ARCHITECTURE: Manually creating a scope to access Scoped database services
            using (var scope = _serviceProvider.CreateScope())
            {
                // Resolve the UnitOfWork from this temporary scope
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                // Query the database for any holds where the expiration has passed
                var expiredHolds = unitOfWork.SeatHold.GetAll(h => h.HoldExpiration <= DateTime.Now).ToList();

                if (expiredHolds.Any())
                {
                    _logger.LogInformation($"Found {expiredHolds.Count} expired seat holds. Wiping from database...");

                    unitOfWork.SeatHold.RemoveRange(expiredHolds);
                    unitOfWork.Save();

                    _logger.LogInformation("Database cleanup complete.");
                }
            }
        }
    }
}
