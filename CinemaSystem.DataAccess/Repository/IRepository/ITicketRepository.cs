using CinemaSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        // Tickets are usually not updated individually; they are recreated or deleted.
        // However, we include the Update method for consistency in the pattern.
        void Update(Ticket obj);
    }
}
