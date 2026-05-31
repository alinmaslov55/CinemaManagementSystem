using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface IFnBProductRepository : IGenericRepository<FnBProduct>
    {
        void Update(FnBProduct obj);
    }
}