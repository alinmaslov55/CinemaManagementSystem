using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        void Update(Booking obj);
        void UpdateStatus(int id, string bookingStatus, string? paymentStatus = null);
        void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
    }
}
