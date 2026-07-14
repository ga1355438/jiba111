using Microsoft.AspNetCore.Mvc;
using LibrarySeatReservation.Web.DataAccess;

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
        return View();
    }

    public async Task<IActionResult> Detail(int id)
    {
        return View();
    }
}
