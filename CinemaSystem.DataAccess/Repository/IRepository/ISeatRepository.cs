using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface ISeatRepository : IGenericRepository<Seat>
    {
        void Update(Seat obj);
    }
}
