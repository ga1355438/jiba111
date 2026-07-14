using Microsoft.AspNetCore.Mvc;
using LibrarySeatReservation.Web.DataAccess;

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

    public async Task<IActionResult> Create(int seatId)
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Models.Entities.Reservation model)
    {
        return RedirectToAction(nameof(My));
    }

    public async Task<IActionResult> My()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        return RedirectToAction(nameof(My));
    }

    [HttpPost]
    public async Task<IActionResult> SwitchUser(string userName)
    {
        HttpContext.Session.SetString("CurrentUserName", userName ?? "张三");
        return RedirectToAction("Index", "Home");
    }
}
