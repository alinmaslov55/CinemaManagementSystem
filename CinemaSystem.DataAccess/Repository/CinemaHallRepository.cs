using CinemaSystem.DataAccess.Data;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.DataAccess.Repository
{
    public class CinemaHallRepository : Repository<CinemaHall>, ICinemaHallRepository
    {
        private readonly ApplicationDbContext _db;

        public CinemaHallRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(CinemaHall obj)
        {
            var objFromDb = _db.CinemaHalls.FirstOrDefault(u => u.Id == obj.Id);

            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.TotalSeats = obj.TotalSeats;
                objFromDb.HallType = obj.HallType;
                objFromDb.CinemaId = obj.CinemaId;

                objFromDb.UpdatedDate = DateTime.Now;
            }
        }
    }
}
