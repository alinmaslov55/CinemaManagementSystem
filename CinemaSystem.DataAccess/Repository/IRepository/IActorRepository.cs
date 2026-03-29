using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface IActorRepository : IGenericRepository<Actor>
    {
        void Update(Actor obj);
    }
}
