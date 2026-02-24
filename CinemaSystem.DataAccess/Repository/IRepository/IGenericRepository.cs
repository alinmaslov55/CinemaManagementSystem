using System.Linq.Expressions;
using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);

        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        void Add(T entity);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);
    }
}