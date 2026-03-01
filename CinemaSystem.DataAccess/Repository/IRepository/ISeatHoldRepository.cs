using CinemaSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface ISeatHoldRepository : IGenericRepository<SeatHold>
    {
        void Update(SeatHold obj);
    }
}
