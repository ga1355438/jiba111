using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.Services;

public interface IReservationService
{
    Task<List<Reservation>> GetReservationsByUserAsync(string userName);
    Task<List<Reservation>> GetAllReservationsAsync();
    Task<bool> CreateReservationAsync(Reservation reservation);
    Task<bool> CancelReservationAsync(int id, string userName);
}
