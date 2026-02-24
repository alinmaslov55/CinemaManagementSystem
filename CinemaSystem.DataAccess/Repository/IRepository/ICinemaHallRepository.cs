using CinemaSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface ICinemaHallRepository : IGenericRepository<CinemaHall>
    {
        void Update(CinemaHall obj);
    }
}
