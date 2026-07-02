using CinemaSystem.DataAccess.Data;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Repository
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        private readonly ApplicationDbContext _db;

        public TicketRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Ticket obj)
        {
            var objFromDb = _db.Tickets.FirstOrDefault(u => u.Id == obj.Id);

            if (objFromDb != null)
            {
                objFromDb.SeatId = obj.SeatId;
                objFromDb.Price = obj.Price;

                objFromDb.IsDeleted = obj.IsDeleted;

                objFromDb.UpdatedDate = DateTime.UtcNow;
            }
        }
    }
}
