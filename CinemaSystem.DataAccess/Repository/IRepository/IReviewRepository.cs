using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        void Update(Review obj);
    }
}
