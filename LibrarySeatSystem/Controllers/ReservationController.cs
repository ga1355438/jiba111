using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibrarySeatSystem.Constants;
using LibrarySeatSystem.Data;
using LibrarySeatSystem.Models.Entities;
using LibrarySeatSystem.Models.Enums;

namespace LibrarySeatSystem.Controllers;

public class ReservationController : Controller
{
    private readonly AppDbContext _context;
    public ReservationController(AppDbContext context) => _context = context;

    private string GetCurrentUserName() => HttpContext.Session.GetString("CurrentUserName") ?? "张三";

    [HttpGet("Reservation/Create/{seatId:int}")]
    public async Task<IActionResult> Create(int seatId)
    {
        var seat = await _context.Seats.FindAsync(seatId);
        if (seat == null || seat.Status != SeatStatus.Available) return NotFound();

        ViewBag.Seat = seat;
        ViewBag.UserName = GetCurrentUserName();
            ViewBag.TimeSlots = TimeSlots.All;
        return View(new Reservation { SeatId = seatId, ReserveDate = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Reservation model)
    {
        var seat = await _context.Seats.FindAsync(model.SeatId);
        if (seat == null || seat.Status != SeatStatus.Available) return NotFound();

        model.UserName = GetCurrentUserName();
        model.Status = ReservationStatus.Reserved;
        model.CreatedAt = DateTime.Now;

        var conflict = await _context.Reservations.AnyAsync(r =>
            r.SeatId == model.SeatId &&
            r.ReserveDate == model.ReserveDate &&
            r.TimeSlot == model.TimeSlot &&
            r.Status == ReservationStatus.Reserved);

        if (conflict)
        {
            ViewBag.Seat = seat;
            ViewBag.UserName = GetCurrentUserName();
        ViewBag.TimeSlots = TimeSlots.All;
            ViewBag.Error = "该时段已被预约，请选择其他时段";
            return View(model);
        }

        _context.Reservations.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(My));
    }

    public async Task<IActionResult> My()
    {
        var userName = GetCurrentUserName();
        var reservations = await _context.Reservations
            .Include(r => r.Seat)
            .Where(r => r.UserName == userName && (r.Status == ReservationStatus.Reserved || r.Status == ReservationStatus.Cancelled))
            .OrderByDescending(r => r.ReserveDate)
            .ThenBy(r => r.TimeSlot)
            .ToListAsync();

        return View(reservations);
    }

    [HttpPost("Reservation/Cancel/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var userName = GetCurrentUserName();
        var reservation = await _context.Reservations.FindAsync(id);

        if (reservation == null || reservation.UserName != userName || reservation.Status != ReservationStatus.Reserved)
            return RedirectToAction(nameof(My));

        reservation.Status = ReservationStatus.Cancelled;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(My));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SwitchUser(string userName)
    {
        HttpContext.Session.SetString("CurrentUserName", userName);
        return RedirectToAction("List", "Seat");
    }
}
