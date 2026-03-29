using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface IMovieRepository : IGenericRepository<Movie>
    {
        void Update(Movie obj);
    }
}
