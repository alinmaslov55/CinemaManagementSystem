using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface IConcessionRepository : IGenericRepository<Concession>
    {
        void Update(Concession obj);
    }
}