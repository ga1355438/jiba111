using Microsoft.EntityFrameworkCore;
using LibrarySeatReservation.Web.Models.Entities;
using LibrarySeatReservation.Web.Data;

namespace LibrarySeatReservation.Web.DataAccess;

public class SeatRepository : ISeatRepository
{
    private readonly AppDbContext _context;

    public SeatRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Seat>> GetAllAsync()
    {
        return await _context.Seats.AsNoTracking().OrderBy(s => s.Id).ToListAsync();
    }

    public async Task<Seat?> GetByIdAsync(int id)
    {
        return await _context.Seats.FindAsync(id);
    }

    public async Task AddAsync(Seat seat)
    {
        _context.Seats.Add(seat);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Seat seat)
    {
        _context.Seats.Update(seat);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var seat = await _context.Seats.FindAsync(id);
        if (seat != null)
        {
            _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        return await _context.Seats.AnyAsync(s => s.Name == name && (!excludeId.HasValue || s.Id != excludeId.Value));
    }

    public async Task<bool> HasReservationsAsync(int id)
    {
        return await _context.Reservations.AnyAsync(r => r.SeatId == id);
    }
}
