using CinemaSystem.DataAccess.Data;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.DataAccess.Repository
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext _db;

        public BookingRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Booking obj)
        {
            var objFromDb = _db.Bookings.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Status = obj.Status;
                objFromDb.PaymentStatus = obj.PaymentStatus;
                objFromDb.TotalAmount = obj.TotalAmount;
                objFromDb.LoyaltyPointsEarned = obj.LoyaltyPointsEarned;
                objFromDb.UpdatedDate = DateTime.Now;
            }
        }

        public void UpdateStatus(int id, string bookingStatus, string? paymentStatus = null)
        {
            var objFromDb = _db.Bookings.FirstOrDefault(u => u.Id == id);
            if (objFromDb != null)
            {
                objFromDb.Status = Enum.Parse<CinemaSystem.Models.Data.Enums.BookingStatus>(bookingStatus);

                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    objFromDb.PaymentStatus = Enum.Parse<CinemaSystem.Models.Data.Enums.PaymentStatus>(paymentStatus);
                }
            }
        }

        public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            var objFromDb = _db.Bookings.FirstOrDefault(u => u.Id == id);
            if (objFromDb != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    // objFromDb.SessionId = sessionId; // Add these to entity later if needed
                }
                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    // objFromDb.PaymentIntentId = paymentIntentId; // Add these to entity later if needed
                }
            }
        }
    }
}
