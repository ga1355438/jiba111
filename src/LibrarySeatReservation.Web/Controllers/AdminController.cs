using Microsoft.AspNetCore.Mvc;
using LibrarySeatReservation.Web.Services;
using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.Controllers;

public class AdminController : Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = await _adminService.ValidateLoginAsync(username, password);
        if (user != null)
        {
            HttpContext.Session.SetString("AdminUsername", user.Username);
            return RedirectToAction(nameof(SeatIndex));
        }
        ViewBag.Error = "用户名或密码错误";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("AdminUsername");
        return RedirectToAction(nameof(Login));
    }

    private bool IsLoggedIn()
    {
        return !string.IsNullOrEmpty(HttpContext.Session.GetString("AdminUsername"));
    }

    public async Task<IActionResult> SeatIndex()
    {
        if (!IsLoggedIn()) return RedirectToAction(nameof(Login));
        return View();
    }

    public async Task<IActionResult> SeatCreate()
    {
        if (!IsLoggedIn()) return RedirectToAction(nameof(Login));
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SeatCreate(Seat seat)
    {
        if (!IsLoggedIn()) return RedirectToAction(nameof(Login));
        return RedirectToAction(nameof(SeatIndex));
    }

    public async Task<IActionResult> SeatEdit(int id)
    {
        if (!IsLoggedIn()) return RedirectToAction(nameof(Login));
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SeatEdit(Seat seat)
    {
        if (!IsLoggedIn()) return RedirectToAction(nameof(Login));
        return RedirectToAction(nameof(SeatIndex));
    }

    [HttpPost]
    public async Task<IActionResult> SeatDelete(int id)
    {
        if (!IsLoggedIn()) return RedirectToAction(nameof(Login));
        return RedirectToAction(nameof(SeatIndex));
    }

    public async Task<IActionResult> ReservationIndex()
    {
        if (!IsLoggedIn()) return RedirectToAction(nameof(Login));
        return View();
    }

    public async Task<IActionResult> Statistics()
    {
        if (!IsLoggedIn()) return RedirectToAction(nameof(Login));
        return View();
    }
}
