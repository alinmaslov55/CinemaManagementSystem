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

        public SeatHoldCleanupService(IServiceProvider serviceProvider, ILogger<SeatHoldCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Seat Hold Cleanup Service is starting.");

            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

            try
            {
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
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

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
