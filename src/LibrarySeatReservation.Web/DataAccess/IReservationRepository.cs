using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.DataAccess;

public interface IReservationRepository
{
    Task<List<Reservation>> GetByUserAsync(string userName);
    Task<List<Reservation>> GetAllAsync();
    Task<List<Reservation>> GetBySeatAndDateAsync(int seatId, DateTime date);
    Task<Reservation?> GetByIdAsync(int id);
    Task AddAsync(Reservation reservation);
    Task UpdateAsync(Reservation reservation);
    Task<bool> HasConflictAsync(int seatId, DateTime date, string timeSlot);
    Task<int> GetTotalCountAsync();
    Task<int> GetTodayCountAsync();
    Task<int> GetActiveCountAsync();
}
