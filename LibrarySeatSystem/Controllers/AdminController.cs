using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibrarySeatSystem.Data;
using LibrarySeatSystem.Models.Entities;
using System.Security.Cryptography;
using System.Text;

namespace LibrarySeatSystem.Controllers;

public class AdminController : Controller
{
    private readonly AppDbContext _context;
    public AdminController(AppDbContext context) => _context = context;

    private bool IsLoggedIn() => HttpContext.Session.GetString("AdminUsername") != null;

    [HttpGet]
    public IActionResult Login()
    {
        if (IsLoggedIn()) return RedirectToAction("SeatIndex");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ViewBag.Error = "请输入用户名和密码";
            return View();
        }

        var hash = ComputeMd5(password);
        var admin = _context.AdminUsers.FirstOrDefault(a =>
            a.Username == username && a.Password == hash);

        if (admin == null)
        {
            ViewBag.Error = "用户名或密码错误";
            return View();
        }

        HttpContext.Session.SetString("AdminUsername", admin.Username);
        return RedirectToAction("SeatIndex");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("AdminUsername");
        return RedirectToAction("Login");
    }

    [HttpGet("Admin/Seat/Index")]
    public async Task<IActionResult> SeatIndex()
    {
        if (!IsLoggedIn()) return RedirectToAction("Login");
        var seats = await _context.Seats.OrderBy(s => s.Id).ToListAsync();
        return View(seats);
    }

    [HttpGet("Admin/SeatCreate")]
    public IActionResult SeatCreate()
    {
        if (!IsLoggedIn()) return RedirectToAction("Login");
        return View(new Seat { Status = 0 });
    }

    [HttpPost("Admin/SeatCreate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SeatCreate(Seat seat)
    {
        if (!IsLoggedIn()) return RedirectToAction("Login");

        if (await _context.Seats.AnyAsync(s => s.Name == seat.Name))
        {
            ViewBag.Error = "座位编号已存在";
            return View(seat);
        }

        seat.CreatedAt = DateTime.Now;
        _context.Seats.Add(seat);
        await _context.SaveChangesAsync();
        return RedirectToAction("SeatIndex");
    }

    [HttpGet("Admin/SeatEdit/{id:int}")]
    public async Task<IActionResult> SeatEdit(int id)
    {
        if (!IsLoggedIn()) return RedirectToAction("Login");
        var seat = await _context.Seats.FindAsync(id);
        if (seat == null) return NotFound();
        return View(seat);
    }

    [HttpPost("Admin/SeatEdit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SeatEdit(Seat seat)
    {
        if (!IsLoggedIn()) return RedirectToAction("Login");

        var existing = await _context.Seats.FindAsync(seat.Id);
        if (existing == null) return NotFound();

        if (await _context.Seats.AnyAsync(s => s.Name == seat.Name && s.Id != seat.Id))
        {
            ViewBag.Error = "座位编号已存在";
            return View(seat);
        }

        existing.Name = seat.Name;
        existing.Location = seat.Location;
        existing.HasPower = seat.HasPower;
        existing.Status = seat.Status;
        await _context.SaveChangesAsync();
        return RedirectToAction("SeatIndex");
    }

    [HttpPost("Admin/SeatDelete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SeatDelete(int id)
    {
        if (!IsLoggedIn()) return RedirectToAction("Login");

        var seat = await _context.Seats.FindAsync(id);
        if (seat == null) return NotFound();

        if (await _context.Reservations.AnyAsync(r => r.SeatId == id))
        {
            var seats = await _context.Seats.OrderBy(s => s.Id).ToListAsync();
            ViewBag.Error = "该座位有预约记录，无法删除";
            return View("SeatIndex", seats);
        }

        _context.Seats.Remove(seat);
        await _context.SaveChangesAsync();
        return RedirectToAction("SeatIndex");
    }

    [HttpGet("Admin/Reservation/Index")]
    public async Task<IActionResult> ReservationIndex(DateTime? date, int? status)
    {
        if (!IsLoggedIn()) return RedirectToAction("Login");

        var query = _context.Reservations.Include(r => r.Seat).AsQueryable();

        if (date.HasValue)
            query = query.Where(r => r.ReserveDate == date.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        var reservations = await query
            .OrderByDescending(r => r.ReserveDate)
            .ThenBy(r => r.TimeSlot)
            .ToListAsync();

        ViewBag.FilterDate = date?.ToString("yyyy-MM-dd");
        ViewBag.FilterStatus = status;
        return View(reservations);
    }

    [HttpGet("Admin/Statistics/Index")]
    public async Task<IActionResult> Statistics()
    {
        if (!IsLoggedIn()) return RedirectToAction("Login");

        var today = DateTime.Today;
        ViewBag.TotalCount = await _context.Reservations.CountAsync();
        ViewBag.TodayCount = await _context.Reservations.CountAsync(r => r.ReserveDate == today);
        ViewBag.ActiveCount = await _context.Reservations.CountAsync(r => r.Status == 0);

        ViewBag.TopSeats = await _context.Reservations
            .GroupBy(r => r.SeatId)
            .Select(g => new { SeatId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .Join(_context.Seats, x => x.SeatId, s => s.Id, (x, s) => new { s.Name, x.Count })
            .ToListAsync();

        var totalRes = await _context.Reservations.CountAsync();
        ViewBag.TimeSlotDistribution = await _context.Reservations
            .GroupBy(r => r.TimeSlot)
            .Select(g => new { Slot = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        ViewBag.TotalForPercentage = totalRes;
        return View();
    }

    private static string ComputeMd5(string input)
    {
        using var md5 = MD5.Create();
        var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
}
