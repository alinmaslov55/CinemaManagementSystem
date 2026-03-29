using CinemaSystem.DataAccess.Data;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Repository
{
    public class EquipmentRepository : Repository<Equipment>, IEquipmentRepository
    {
        private ApplicationDbContext _db;

        public EquipmentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Equipment obj)
        {
            _db.Equipments.Update(obj);
        }
    }
}
