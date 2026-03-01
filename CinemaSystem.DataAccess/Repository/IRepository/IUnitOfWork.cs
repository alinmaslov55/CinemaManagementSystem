using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICinemaRepository Cinema { get; }
        ICinemaHallRepository CinemaHall { get; }
        ISeatRepository Seat { get; }
        IMovieRepository Movie { get; }
        IActorRepository Actor { get; }
        IShowtimeRepository Showtime { get; }
        IBookingRepository Booking { get; }
        ITicketRepository Ticket { get; }
        IReviewRepository Review { get; }
        ISeatHoldRepository SeatHold { get; }

        void Save();
    }
}
