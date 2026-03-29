using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface IShowtimeRepository : IGenericRepository<Showtime>
    {
        void Update(Showtime obj);
    }
}
