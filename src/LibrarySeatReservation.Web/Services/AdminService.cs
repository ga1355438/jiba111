using LibrarySeatReservation.Web.Models.Entities;
using LibrarySeatReservation.Web.Helpers;

namespace LibrarySeatReservation.Web.Services;

public class AdminService : IAdminService
{
    private readonly DataAccess.IAdminUserRepository _adminUserRepository;

    public AdminService(DataAccess.IAdminUserRepository adminUserRepository)
    {
        _adminUserRepository = adminUserRepository;
    }

    public async Task<AdminUser?> ValidateLoginAsync(string username, string password)
    {
        var user = await _adminUserRepository.GetByUsernameAsync(username);
        if (user == null) return null;

        if (PasswordHelper.VerifyPassword(user.Password, password))
            return user;

        return null;
    }
}
