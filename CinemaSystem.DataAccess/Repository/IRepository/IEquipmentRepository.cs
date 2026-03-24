using CinemaSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface IEquipmentRepository : IGenericRepository<Equipment>
    {
        void Update(Equipment obj);
    }
}
