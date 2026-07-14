using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using LibrarySeatReservation.Web.Services;
using LibrarySeatReservation.Web.DataAccess;
using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.Controllers;

public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly ISeatRepository _seatRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IMemoryCache _cache;

    public AdminController(
        IAdminService adminService,
        ISeatRepository seatRepository,
        IReservationRepository reservationRepository,
        IMemoryCache cache)
    {
        _adminService = adminService;
        _seatRepository = seatRepository;
        _reservationRepository = reservationRepository;
        _cache = cache;
    }

    public IActionResult Login() => View();

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

    private bool IsLoggedIn() => !string.IsNullOrEmpty(HttpContext.Session.GetString("AdminUsername"));

    // ========== 座位管理 ==========

    public async Task<IActionResult> SeatIndex()
    {
        if (!IsLoggedIn()) return RedirectToAction(nameof(Login));
        var seats = await _seatRepository.GetAllAsync();
        return View(seats);
    }

    public IActionResult SeatCreate()
    {
        if (!IsLoggedIn()) return RedirectToAction(nameof(Login));
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SeatCreate(Seat seat)
    {
        if (!IsLoggedIn()) return RedirectToAction(nameof(Login));

        if (await _seatRepository.ExistsByNameAsync(seat.Name))
        {
            ViewBag.Error = "座位编号已存在";
            return View(seat);
        }

        seat.CreatedAt = DateTime.UtcNow;
        await _seatRepository.AddAsync(seat);
        return RedirectToAction(nameof(SeatIndex));
    }

    public async Task<IActionResult> SeatEdit(int id)
    {
        if (!IsLoggedIn()) return RedirectToAction(nameof(Login));
        var seat = await _seatRepository.GetByIdAsync(id);
        if (seat == null) return NotFound();
        return View(seat);
    }

    [HttpPost]
    public async Task<IActionResult> SeatEdit(Seat seat)
    {
        if (!IsLoggedIn()) return RedirectToAction(nameof(Login));
        var existing = await _seatRepository.GetByIdAsync(seat.Id);
        if (existing == null) return NotFound();

        existing.Name = seat.Name;
        existing.Location = seat.Location;
        existing.HasPower = seat.HasPower;
        existing.Status = seat.Status;
        await _seatRepository.UpdateAsync(existing);
        return RedirectToAction(nameof(SeatIndex));
    }

    [HttpPost]
    public async Task<IActionResult> SeatDelete(int id)
    {
        if (!IsLoggedIn()) return RedirectToAction(nameof(Login));
        var seat = await _seatRepository.GetByIdAsync(id);
        if (seat == null) return NotFound();

        if (await _seatRepository.HasReservationsAsync(id))
        {
            TempData["Error"] = "该座位有预约记录，无法删除";
            return RedirectToAction(nameof(SeatIndex));
        }

        await _seatRepository.DeleteAsync(id);
        return RedirectToAction(nameof(SeatIndex));
    }

    // ========== 预约管理 ==========

    public async Task<IActionResult> ReservationIndex(DateTime? date, int? status, int page = 1)
    {
        if (!IsLoggedIn()) return RedirectToAction(nameof(Login));

        var reservations = await _reservationRepository.GetAllAsync();

        if (date.HasValue)
            reservations = reservations.Where(r => r.ReserveDate.Date == date.Value.Date).ToList();

        if (status.HasValue)
            reservations = reservations.Where(r => r.Status == status.Value).ToList();

        reservations = reservations.OrderByDescending(r => r.ReserveDate).ToList();

        int pageSize = 15;
        int totalItems = reservations.Count;
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        var paged = reservations.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalItems = totalItems;
        ViewBag.FilterDate = date?.ToString("yyyy-MM-dd");
        ViewBag.FilterStatus = status;

        return View(paged);
    }

    // ========== 统计 ==========

    public async Task<IActionResult> Statistics()
    {
        if (!IsLoggedIn()) return RedirectToAction(nameof(Login));

        var cacheKey = "StatisticsData";
        object? stats;
        if (!_cache.TryGetValue(cacheKey, out stats))
        {
            var allReservations = await _reservationRepository.GetAllAsync();
            var seats = await _seatRepository.GetAllAsync();
            var seatDict = seats.ToDictionary(s => s.Id, s => s.Name);

            stats = new
            {
                TotalCount = allReservations.Count,
                TodayCount = allReservations.Count(r => r.ReserveDate.Date == DateTime.Today && r.Status != 2),
                ActiveCount = allReservations.Count(r => r.Status == 0),
                TopSeats = allReservations
                    .GroupBy(r => r.SeatId)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => new { SeatId = g.Key, SeatName = seatDict.GetValueOrDefault(g.Key, $"座位{g.Key}"), Count = g.Count() })
                    .ToList(),
                TimeSlotDist = allReservations
                    .GroupBy(r => r.TimeSlot)
                    .OrderBy(g => g.Key)
                    .Select(g => new { Slot = g.Key, Count = g.Count() })
                    .ToList()
            };

            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            _cache.Set(cacheKey, stats, options);
        }

        ViewBag.Stats = stats;
        return View();
    }
}
