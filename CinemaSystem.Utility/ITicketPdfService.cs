using CinemaSystem.Models.Entities;

namespace CinemaSystem.Utility
{
    public interface ITicketPdfService
    {
        byte[] GenerateTicketPdfBytes(Booking booking);
    }
}