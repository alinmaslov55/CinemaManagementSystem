using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface ICinemaRepository : IGenericRepository<Cinema>
    {
        void Update(Cinema obj);
    }
}