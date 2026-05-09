using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CinemaSystem.Web.BackgroundServices;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;

namespace CinemaSystem.Tests.BackgroundServices
{
    public class SeatHoldCleanupServiceTests
    {
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
        private readonly Mock<IServiceScope> _mockScope;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger<SeatHoldCleanupService>> _mockLogger;
        private readonly SeatHoldCleanupService _service;

        public SeatHoldCleanupServiceTests()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockScopeFactory = new Mock<IServiceScopeFactory>();
            _mockScope = new Mock<IServiceScope>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<SeatHoldCleanupService>>();

            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(_mockScopeFactory.Object);

            _mockScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(_mockScope.Object);

            _mockScope
                .Setup(x => x.ServiceProvider)
                .Returns(_mockServiceProvider.Object);

            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IUnitOfWork)))
                .Returns(_mockUnitOfWork.Object);

            _service = new SeatHoldCleanupService(_mockServiceProvider.Object, _mockLogger.Object);
        }

        [Fact]
        public void CleanUpExpiredHolds_DoesNothing_WhenNoHoldsAreExpired()
        {
            _mockUnitOfWork.Setup(u => u.SeatHold.GetAll(It.IsAny<Expression<Func<SeatHold, bool>>>(), It.IsAny<string>()))
                           .Returns(new List<SeatHold>());

            _service.CleanUpExpiredHolds();

            _mockUnitOfWork.Verify(u => u.SeatHold.RemoveRange(It.IsAny<IEnumerable<SeatHold>>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void CleanUpExpiredHolds_RemovesHoldsAndSaves_WhenExpiredHoldsExist()
        {
            var expiredHolds = new List<SeatHold>
            {
                new SeatHold { Id = 1, HoldExpiration = DateTime.Now.AddMinutes(-5) },
                new SeatHold { Id = 2, HoldExpiration = DateTime.Now.AddMinutes(-10) }
            };

            _mockUnitOfWork.Setup(u => u.SeatHold.GetAll(It.IsAny<Expression<Func<SeatHold, bool>>>(), It.IsAny<string>()))
                           .Returns(expiredHolds);

            _service.CleanUpExpiredHolds();

            _mockUnitOfWork.Verify(u => u.SeatHold.RemoveRange(It.Is<IEnumerable<SeatHold>>(holds => holds.Count() == 2)), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
        }
    }
}