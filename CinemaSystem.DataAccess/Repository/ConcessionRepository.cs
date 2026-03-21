using CinemaSystem.DataAccess.Data;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Repository
{
    public class ConcessionRepository : Repository<Concession>, IConcessionRepository
    {
        private ApplicationDbContext _db;

        public ConcessionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Concession obj)
        {
            _db.Concessions.Update(obj);
        }
    }
}