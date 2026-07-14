using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibrarySeatSystem.Data;
using LibrarySeatSystem.Models.Enums;

namespace LibrarySeatSystem.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    public HomeController(AppDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalSeats = await _context.Seats.CountAsync();
        ViewBag.AvailableSeats = await _context.Seats.CountAsync(s => s.Status == SeatStatus.Available);
        ViewBag.MaintenanceSeats = await _context.Seats.CountAsync(s => s.Status == SeatStatus.UnderMaintenance);

        return View();
    }
}
