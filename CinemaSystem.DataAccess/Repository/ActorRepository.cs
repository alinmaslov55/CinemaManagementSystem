using CinemaSystem.DataAccess.Data;
using CinemaSystem.DataAccess.Repository.IRepository;
using CinemaSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.DataAccess.Repository
{
    public class ActorRepository : Repository<Actor>, IActorRepository
    {
        private readonly ApplicationDbContext _db;

        public ActorRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Actor obj)
        {
            var objFromDb = _db.Actors.FirstOrDefault(u => u.Id == obj.Id);

            if (objFromDb != null)
            {
                objFromDb.FullName = obj.FullName;
                objFromDb.Bio = obj.Bio;

                objFromDb.UpdatedDate = DateTime.Now;

                if (obj.ProfilePictureURL != null)
                {
                    objFromDb.ProfilePictureURL = obj.ProfilePictureURL;
                }
            }
        }
    }
}
