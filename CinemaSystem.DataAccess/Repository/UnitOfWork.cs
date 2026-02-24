using CinemaSystem.DataAccess.Data;
using CinemaSystem.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        // Repositories are public so they can be accessed via UnitOfWork.Entity
        public ICinemaRepository Cinema { get; private set; }
        public ICinemaHallRepository CinemaHall { get; private set; }
        public ISeatRepository Seat { get; private set; }
        public IMovieRepository Movie { get; private set; }
        public IActorRepository Actor { get; private set; }
        public IShowtimeRepository Showtime { get; private set; }
        public IBookingRepository Booking { get; private set; }
        public ITicketRepository Ticket { get; private set; }
        public IReviewRepository Review { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            Cinema = new CinemaRepository(_db);
            CinemaHall = new CinemaHallRepository(_db);
            Seat = new SeatRepository(_db);
            Movie = new MovieRepository(_db);
            Actor = new ActorRepository(_db);
            Showtime = new ShowtimeRepository(_db);
            Booking = new BookingRepository(_db);
            Ticket = new TicketRepository(_db);
            Review = new ReviewRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
