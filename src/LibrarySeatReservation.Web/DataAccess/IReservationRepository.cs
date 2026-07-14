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
    Task<int> GetFilteredCountAsync(DateTime? date, int? status);
    Task<List<Reservation>> GetFilteredPageAsync(DateTime? date, int? status, int skip, int take);
    Task<List<(int SeatId, int Count)>> GetTopSeatsAsync(int take);
    Task<List<(string TimeSlot, int Count)>> GetTimeSlotDistAsync();
}
