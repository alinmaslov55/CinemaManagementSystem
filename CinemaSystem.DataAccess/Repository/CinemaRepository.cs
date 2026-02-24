using CinemaSystem.DataAccess.Data;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.DataAccess.Repository
{
    public class CinemaRepository : Repository<Cinema>, ICinemaRepository
    {
        private readonly ApplicationDbContext _db;

        public CinemaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Cinema obj)
        {
            var objFromDb = _db.Cinemas.FirstOrDefault(u => u.Id == obj.Id);

            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.Address = obj.Address;
                objFromDb.Description = obj.Description;

                objFromDb.UpdatedDate = DateTime.Now;

                if (obj.Logo != null)
                {
                    objFromDb.Logo = obj.Logo;
                }
            }
        }
    }
}
