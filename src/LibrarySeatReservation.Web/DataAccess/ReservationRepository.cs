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
            .Where(r => r.UserName == userName)
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
}
