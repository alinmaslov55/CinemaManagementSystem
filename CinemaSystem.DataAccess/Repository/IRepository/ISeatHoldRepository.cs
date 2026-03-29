using CinemaSystem.Models.Entities;

namespace CinemaSystem.DataAccess.Repository.IRepository
{
    public interface ISeatHoldRepository : IGenericRepository<SeatHold>
    {
        void Update(SeatHold obj);
    }
}
