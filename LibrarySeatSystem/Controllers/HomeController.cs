using Microsoft.AspNetCore.Mvc;
using LibrarySeatSystem.Data;
using LibrarySeatSystem.Models.Entities;

namespace LibrarySeatSystem.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    public HomeController(AppDbContext context) => _context = context;

    public IActionResult Index()
    {
        return View();
    }
}
