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
            var objFromDb = _db.Equipments.FirstOrDefault(u => u.Id == obj.Id);

            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.SerialNumber = obj.SerialNumber;
                objFromDb.Type = obj.Type;
                objFromDb.Status = obj.Status;
                objFromDb.PurchaseDate = obj.PurchaseDate;
                objFromDb.LastMaintenanceDate = obj.LastMaintenanceDate;
                objFromDb.NextMaintenanceDate = obj.NextMaintenanceDate;
                objFromDb.MaintenanceNotes = obj.MaintenanceNotes;
                objFromDb.CinemaHallId = obj.CinemaHallId;

                objFromDb.IsDeleted = obj.IsDeleted;

                objFromDb.UpdatedDate = DateTime.UtcNow;
            }
        }
    }
}
