using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface IEquipmentRepository : IGenericRepository<Equipment>
    {
        void Update(Equipment obj);
    }
}
