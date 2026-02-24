using CinemaSystem.DataAccess.Data;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.DataAccess.Repository
{
    public class ShowtimeRepository : Repository<Showtime>, IShowtimeRepository
    {
        private readonly ApplicationDbContext _db;

        public ShowtimeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Showtime obj)
        {
            var objFromDb = _db.Showtimes.FirstOrDefault(u => u.Id == obj.Id);

            if (objFromDb != null)
            {
                objFromDb.MovieId = obj.MovieId;
                objFromDb.CinemaHallId = obj.CinemaHallId;
                objFromDb.StartTime = obj.StartTime;
                objFromDb.EndTime = obj.EndTime;
                objFromDb.Price = obj.Price;
                objFromDb.Language = obj.Language;
                objFromDb.Subtitles = obj.Subtitles;

                objFromDb.UpdatedDate = DateTime.Now;
            }
        }
    }
}
