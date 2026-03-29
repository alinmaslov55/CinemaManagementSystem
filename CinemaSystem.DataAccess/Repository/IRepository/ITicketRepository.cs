using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        void Update(Ticket obj);
    }
}
