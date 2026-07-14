using Microsoft.EntityFrameworkCore;
using LibrarySeatReservation.Web.Models.Entities;
using LibrarySeatReservation.Web.Data;

namespace LibrarySeatReservation.Web.DataAccess;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _context;

    public ReservationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Reservation>> GetByUserAsync(string userName)
    {
        return await _context.Reservations
            .AsNoTracking()
            .Include(r => r.Seat)
            .Where(r => r.UserName == userName && (r.Status == 0 || r.Status == 2))
            .OrderByDescending(r => r.ReserveDate)
            .ToListAsync();
    }

    public async Task<List<Reservation>> GetAllAsync()
    {
        return await _context.Reservations
            .AsNoTracking()
            .Include(r => r.Seat)
            .OrderByDescending(r => r.ReserveDate)
            .ToListAsync();
    }

    public async Task<List<Reservation>> GetBySeatAndDateAsync(int seatId, DateTime date)
    {
        return await _context.Reservations
            .AsNoTracking()
            .Where(r => r.SeatId == seatId && r.ReserveDate.Date == date.Date && r.Status != 2)
            .ToListAsync();
    }

    public async Task<Reservation?> GetByIdAsync(int id)
    {
        return await _context.Reservations.AsNoTracking().Include(r => r.Seat).FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task AddAsync(Reservation reservation)
    {
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Reservation reservation)
    {
        _context.Reservations.Update(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasConflictAsync(int seatId, DateTime date, string timeSlot)
    {
        return await _context.Reservations.AnyAsync(r =>
            r.SeatId == seatId &&
            r.ReserveDate.Date == date.Date &&
            r.TimeSlot == timeSlot &&
            r.Status == 0);
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Reservations.CountAsync();
    }

    public async Task<int> GetTodayCountAsync()
    {
        return await _context.Reservations.CountAsync(r =>
            r.ReserveDate.Date == DateTime.Today && r.Status != 2);
    }

    public async Task<int> GetActiveCountAsync()
    {
        return await _context.Reservations.CountAsync(r => r.Status == 0);
    }

    public async Task<int> GetFilteredCountAsync(DateTime? date, int? status)
    {
        var query = _context.Reservations.AsNoTracking().AsQueryable();
        if (date.HasValue)
            query = query.Where(r => r.ReserveDate.Date == date.Value.Date);
        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);
        return await query.CountAsync();
    }

    public async Task<List<Reservation>> GetFilteredPageAsync(DateTime? date, int? status, int skip, int take)
    {
        var query = _context.Reservations
            .AsNoTracking()
            .Include(r => r.Seat)
            .AsQueryable();
        if (date.HasValue)
            query = query.Where(r => r.ReserveDate.Date == date.Value.Date);
        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);
        return await query
            .OrderByDescending(r => r.ReserveDate)
            .Skip(skip).Take(take)
            .ToListAsync();
    }

    public async Task<List<(int SeatId, int Count)>> GetTopSeatsAsync(int take)
    {
        return await _context.Reservations
            .AsNoTracking()
            .Where(r => r.Status != 2)
            .GroupBy(r => r.SeatId)
            .OrderByDescending(g => g.Count())
            .Take(take)
            .Select(g => new ValueTuple<int, int>(g.Key, g.Count()))
            .ToListAsync();
    }

    public async Task<List<(string TimeSlot, int Count)>> GetTimeSlotDistAsync()
    {
        return await _context.Reservations
            .AsNoTracking()
            .Where(r => r.Status != 2)
            .GroupBy(r => r.TimeSlot)
            .OrderBy(g => g.Key)
            .Select(g => new ValueTuple<string, int>(g.Key, g.Count()))
            .ToListAsync();
    }
}
