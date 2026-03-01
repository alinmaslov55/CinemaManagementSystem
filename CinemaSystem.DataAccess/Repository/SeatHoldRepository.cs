using CinemaSystem.DataAccess.Data;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.DataAccess.Repository
{
    public class SeatHoldRepository : Repository<SeatHold>, ISeatHoldRepository
    {
        private ApplicationDbContext _db;

        public SeatHoldRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(SeatHold obj)
        {
            _db.SeatHolds.Update(obj);
        }
    }
}
