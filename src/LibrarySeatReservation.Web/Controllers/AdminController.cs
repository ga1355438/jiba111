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

        try
        {
            seat.CreatedAt = DateTime.UtcNow;
            await _seatRepository.AddAsync(seat);
            return RedirectToAction(nameof(SeatIndex));
        }
        catch
        {
            ViewBag.Error = "操作失败，请稍后重试";
            return View(seat);
        }
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

        try
        {
            await _seatRepository.UpdateAsync(existing);
            return RedirectToAction(nameof(SeatIndex));
        }
        catch
        {
            ViewBag.Error = "操作失败，请稍后重试";
            return View(existing);
        }
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

        try
        {
            await _seatRepository.DeleteAsync(id);
            return RedirectToAction(nameof(SeatIndex));
        }
        catch
        {
            TempData["Error"] = "删除失败，请稍后重试";
            return RedirectToAction(nameof(SeatIndex));
        }
    }

    // ========== 预约管理 ==========

    public async Task<IActionResult> ReservationIndex(DateTime? date, int? status, int page = 1)
    {
        if (!IsLoggedIn()) return RedirectToAction(nameof(Login));

        int pageSize = 15;
        int totalItems = await _reservationRepository.GetFilteredCountAsync(date, status);
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        if (page < 1) page = 1;
        if (page > totalPages && totalPages > 0) page = totalPages;

        var paged = await _reservationRepository.GetFilteredPageAsync(date, status, (page - 1) * pageSize, pageSize);

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
            var topSeats = await _reservationRepository.GetTopSeatsAsync(5);
            var timeSlotDist = await _reservationRepository.GetTimeSlotDistAsync();
            var seats = await _seatRepository.GetAllAsync();
            var seatDict = seats.ToDictionary(s => s.Id, s => s.Name);

            stats = new
            {
                TotalCount = await _reservationRepository.GetTotalCountAsync(),
                TodayCount = await _reservationRepository.GetTodayCountAsync(),
                ActiveCount = await _reservationRepository.GetActiveCountAsync(),
                TopSeats = topSeats.Select(t => new { SeatId = t.SeatId, SeatName = seatDict.GetValueOrDefault(t.SeatId, $"座位{t.SeatId}"), Count = t.Count }).ToList(),
                TimeSlotDist = timeSlotDist.Select(t => new { Slot = t.TimeSlot, Count = t.Count }).ToList()
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
