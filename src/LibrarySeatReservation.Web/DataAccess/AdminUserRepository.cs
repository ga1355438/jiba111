using Microsoft.EntityFrameworkCore;
using LibrarySeatReservation.Web.Models.Entities;
using LibrarySeatReservation.Web.Data;

namespace LibrarySeatReservation.Web.DataAccess;

public class AdminUserRepository : IAdminUserRepository
{
    private readonly AppDbContext _context;

    public AdminUserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AdminUser?> GetByUsernameAsync(string username)
    {
        return await _context.AdminUsers.FirstOrDefaultAsync(a => a.Username == username);
    }
}
