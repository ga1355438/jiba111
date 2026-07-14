using Microsoft.AspNetCore.Mvc;
using LibrarySeatReservation.Web.DataAccess;
using LibrarySeatReservation.Web.Models.Entities;
using LibrarySeatReservation.Web.Constants;

namespace LibrarySeatReservation.Web.Controllers;

public class ReservationController : Controller
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ISeatRepository _seatRepository;

    public ReservationController(IReservationRepository reservationRepository, ISeatRepository seatRepository)
    {
        _reservationRepository = reservationRepository;
        _seatRepository = seatRepository;
    }

    public async Task<IActionResult> Create(int seatId, string? timeSlot)
    {
        var seat = await _seatRepository.GetByIdAsync(seatId);
        if (seat == null) return NotFound();

        ViewBag.Seat = seat;
        ViewBag.TimeSlots = TimeSlots.All;
        ViewBag.DisplayNames = TimeSlots.DisplayNames;
        ViewBag.SelectedTimeSlot = timeSlot;
        ViewBag.Today = DateTime.Today;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(int seatId, DateTime reserveDate, string timeSlot)
    {
        var seat = await _seatRepository.GetByIdAsync(seatId);
        if (seat == null) return NotFound();

        var userName = HttpContext.Session.GetString("CurrentUserName") ?? "张三";

        bool hasConflict = await _reservationRepository.HasConflictAsync(seatId, reserveDate, timeSlot);
        if (hasConflict)
        {
            ViewBag.Seat = seat;
            ViewBag.TimeSlots = TimeSlots.All;
            ViewBag.DisplayNames = TimeSlots.DisplayNames;
            ViewBag.SelectedTimeSlot = timeSlot;
            ViewBag.Today = DateTime.Today;
            ViewBag.Error = "该时段已被预约";
            return View();
        }

        var reservation = new Reservation
        {
            SeatId = seatId,
            UserName = userName,
            ReserveDate = reserveDate.Date,
            TimeSlot = timeSlot,
            Status = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _reservationRepository.AddAsync(reservation);
        return RedirectToAction(nameof(My));
    }

    public async Task<IActionResult> My()
    {
        var userName = HttpContext.Session.GetString("CurrentUserName") ?? "张三";
        var reservations = await _reservationRepository.GetByUserAsync(userName);
        ViewBag.CurrentUserName = userName;
        return View(reservations);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        var userName = HttpContext.Session.GetString("CurrentUserName") ?? "张三";
        var reservation = await _reservationRepository.GetByIdAsync(id);

        if (reservation == null) return NotFound();
        if (reservation.UserName != userName) return Forbid();
        if (reservation.Status != 0) return BadRequest();

        reservation.Status = 2;
        await _reservationRepository.UpdateAsync(reservation);

        return RedirectToAction(nameof(My));
    }

    [HttpPost]
    public IActionResult SwitchUser(string userName)
    {
        HttpContext.Session.SetString("CurrentUserName", userName ?? "张三");
        return RedirectToAction("Index", "Home");
    }
}
