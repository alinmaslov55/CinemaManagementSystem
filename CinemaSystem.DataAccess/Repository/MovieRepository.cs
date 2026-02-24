using CinemaSystem.DataAccess.Data;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.DataAccess.Repository
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly ApplicationDbContext _db;

        public MovieRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Movie obj)
        {
            var objFromDb = _db.Movies.FirstOrDefault(u => u.Id == obj.Id);

            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.Description = obj.Description;
                objFromDb.Price = obj.Price;
                objFromDb.StartDate = obj.StartDate;
                objFromDb.EndDate = obj.EndDate;
                objFromDb.DurationInMinutes = obj.DurationInMinutes;
                objFromDb.MovieCategory = obj.MovieCategory;
                objFromDb.AgeRating = obj.AgeRating;
                objFromDb.IsReleased = obj.IsReleased;

                objFromDb.UpdatedDate = DateTime.Now;

                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }

                if (obj.TrailerUrl != null)
                {
                    objFromDb.TrailerUrl = obj.TrailerUrl;
                }
            }
        }
    }
}
