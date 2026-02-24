using CinemaSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface ISeatRepository : IGenericRepository<Seat>
    {
        void Update(Seat obj);
    }
}
