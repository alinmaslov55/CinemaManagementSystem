using CinemaSystem.DataAccess.Data;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.DataAccess.Repository
{
    public class SeatRepository : Repository<Seat>, ISeatRepository
    {
        private readonly ApplicationDbContext _db;

        public SeatRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Seat obj)
        {
            var objFromDb = _db.Seats.FirstOrDefault(u => u.Id == obj.Id);

            if (objFromDb != null)
            {
                objFromDb.Row = obj.Row;
                objFromDb.Column = obj.Column;
                objFromDb.SeatType = obj.SeatType;
                objFromDb.IsAccessible = obj.IsAccessible;
                objFromDb.CinemaHallId = obj.CinemaHallId;

                objFromDb.UpdatedDate = DateTime.Now;
            }
        }
    }
}
