using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface ICinemaHallRepository : IGenericRepository<CinemaHall>
    {
        void Update(CinemaHall obj);
    }
}
