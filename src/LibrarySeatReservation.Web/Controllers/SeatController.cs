using Microsoft.AspNetCore.Mvc;
using LibrarySeatReservation.Web.DataAccess;
using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.Controllers;

public class SeatController : Controller
{
    private readonly ISeatRepository _seatRepository;
    private readonly IReservationRepository _reservationRepository;

    public SeatController(ISeatRepository seatRepository, IReservationRepository reservationRepository)
    {
        _seatRepository = seatRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<IActionResult> List(int page = 1)
    {
        int pageSize = 10;
        var seats = await _seatRepository.GetAllAsync();
        int totalItems = seats.Count;
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var pagedSeats = seats
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalItems = totalItems;

        return View(pagedSeats);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var seat = await _seatRepository.GetByIdAsync(id);
        if (seat == null) return NotFound();

        var today = DateTime.Today;
        var reservations = await _reservationRepository.GetBySeatAndDateAsync(id, today);

        ViewBag.Reservations = reservations;
        ViewBag.Today = today;

        return View(seat);
    }
}
