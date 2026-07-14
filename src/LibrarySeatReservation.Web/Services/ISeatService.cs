using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.Services;

public interface ISeatService
{
    Task<List<Seat>> GetAllSeatsAsync();
    Task<Seat?> GetSeatByIdAsync(int id);
    Task<bool> CreateSeatAsync(Seat seat);
    Task<bool> UpdateSeatAsync(Seat seat);
    Task<bool> DeleteSeatAsync(int id);
}
