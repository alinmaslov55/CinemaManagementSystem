using CinemaSystem.DataAccess.Data;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Repository
{
    public class FnBProductRepository : Repository<FnBProduct>, IFnBProductRepository
    {
        private ApplicationDbContext _db;

        public FnBProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(FnBProduct obj)
        {
            var objFromDb = _db.FnBProducts.FirstOrDefault(u => u.Id == obj.Id);

            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.Description = obj.Description;
                objFromDb.Price = obj.Price;
                objFromDb.Category = obj.Category;
                objFromDb.IsActive = obj.IsActive;

                objFromDb.IsDeleted = obj.IsDeleted;
                objFromDb.UpdatedDate = DateTime.UtcNow;

                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}