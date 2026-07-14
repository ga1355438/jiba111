using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibrarySeatSystem.Data;
using LibrarySeatSystem.Models.Entities;

namespace LibrarySeatSystem.Controllers;

public class SeatController : Controller
{
    private readonly AppDbContext _context;
    public SeatController(AppDbContext context) => _context = context;

    public async Task<IActionResult> List(int page = 1)
    {
        int pageSize = 10;
        var total = await _context.Seats.CountAsync();
        var seats = await _context.Seats
            .OrderBy(s => s.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        return View(seats);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var seat = await _context.Seats.FindAsync(id);
        if (seat == null) return NotFound();

        var today = DateTime.Today;
        var reservations = await _context.Reservations
            .Where(r => r.SeatId == id && r.ReserveDate == today && r.Status == 0)
            .Select(r => r.TimeSlot)
            .ToListAsync();

        ViewBag.ReservedSlots = reservations;
        ViewBag.Today = today;
        return View(seat);
    }
}
