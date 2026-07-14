using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.DataAccess;

public interface ISeatRepository
{
    Task<List<Seat>> GetAllAsync();
    Task<Seat?> GetByIdAsync(int id);
    Task AddAsync(Seat seat);
    Task UpdateAsync(Seat seat);
    Task DeleteAsync(int id);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    Task<bool> HasReservationsAsync(int id);
}
