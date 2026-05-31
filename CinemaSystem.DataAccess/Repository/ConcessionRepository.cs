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
            _db.FnBProducts.Update(obj);
        }
    }
}