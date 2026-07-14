using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using LibrarySeatSystem.Data;
using LibrarySeatSystem.Filters;
using LibrarySeatSystem.Helpers;
using LibrarySeatSystem.Models.Entities;
using LibrarySeatSystem.Models.Enums;

namespace LibrarySeatSystem.Controllers;

[ServiceFilter(typeof(AdminAuthorizationFilter))]
public class AdminController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;
    public AdminController(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetString("AdminUsername") != null)
            return RedirectToAction("SeatIndex");
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult Login(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ViewBag.Error = "请输入用户名和密码";
            return View();
        }

        var hash = PasswordHelper.HashPassword(password);
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

    [AllowAnonymous]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("AdminUsername");
        return RedirectToAction("Login");
    }

    [HttpGet("Admin/Seat/Index")]
    public async Task<IActionResult> SeatIndex()
    {
        var seats = await _context.Seats.AsNoTracking().OrderBy(s => s.Id).ToListAsync();
        return View(seats);
    }

    [HttpGet("Admin/SeatCreate")]
    public IActionResult SeatCreate()
    {
        return View(new Seat { Status = SeatStatus.Available });
    }

    [HttpPost("Admin/SeatCreate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SeatCreate(Seat seat)
    {
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
        var seat = await _context.Seats.FindAsync(id);
        if (seat == null) return NotFound();
        return View(seat);
    }

    [HttpPost("Admin/SeatEdit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SeatEdit(Seat seat)
    {
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
    public async Task<IActionResult> ReservationIndex(DateTime? date, ReservationStatus? status, int page = 1)
    {
        int pageSize = 10;
        var query = _context.Reservations.Include(r => r.Seat).AsNoTracking().AsQueryable();

        if (date.HasValue)
            query = query.Where(r => r.ReserveDate == date.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        var total = await query.CountAsync();
        var reservations = await query
            .OrderByDescending(r => r.ReserveDate)
            .ThenBy(r => r.TimeSlot)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.FilterDate = date?.ToString("yyyy-MM-dd");
        ViewBag.FilterStatus = status;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        ViewBag.TotalCount = total;
        return View(reservations);
    }

    [HttpGet("Admin/Statistics/Index")]
    public async Task<IActionResult> Statistics()
    {
        var cacheKey = "StatisticsData";
        if (!_cache.TryGetValue(cacheKey, out StatisticsData stats))
        {
            var today = DateTime.Today;
            var reservations = await _context.Reservations
                .Include(r => r.Seat)
                .AsNoTracking()
                .ToListAsync();

            stats = new StatisticsData
            {
                TotalCount = reservations.Count,
                TodayCount = reservations.Count(r => r.ReserveDate.Date == today && r.Status != ReservationStatus.Cancelled),
                ActiveCount = reservations.Count(r => r.Status == ReservationStatus.Reserved),
                TopSeats = reservations
                    .Where(r => r.Status != ReservationStatus.Cancelled)
                    .GroupBy(r => r.SeatId)
                    .Select(g => new { SeatId = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .Join(_context.Seats, x => x.SeatId, s => s.Id, (x, s) => new TopSeatInfo { Name = s.Name, Count = x.Count })
                    .ToList(),
                TimeSlotDistribution = reservations
                    .Where(r => r.Status != ReservationStatus.Cancelled)
                    .GroupBy(r => r.TimeSlot)
                    .Select(g => new TimeSlotInfo { Slot = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ToList(),
                TotalForPercentage = reservations.Count(r => r.Status != ReservationStatus.Cancelled)
            };

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            _cache.Set(cacheKey, stats, cacheOptions);
        }

        ViewBag.TotalCount = stats.TotalCount;
        ViewBag.TodayCount = stats.TodayCount;
        ViewBag.ActiveCount = stats.ActiveCount;
        ViewBag.TopSeats = stats.TopSeats;
        ViewBag.TimeSlotDistribution = stats.TimeSlotDistribution;
        ViewBag.TotalForPercentage = stats.TotalForPercentage;

        return View();
    }
}

public class StatisticsData
{
    public int TotalCount { get; set; }
    public int TodayCount { get; set; }
    public int ActiveCount { get; set; }
    public List<TopSeatInfo> TopSeats { get; set; } = new();
    public List<TimeSlotInfo> TimeSlotDistribution { get; set; } = new();
    public int TotalForPercentage { get; set; }
}

public class TopSeatInfo
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class TimeSlotInfo
{
    public string Slot { get; set; } = string.Empty;
    public int Count { get; set; }
}
