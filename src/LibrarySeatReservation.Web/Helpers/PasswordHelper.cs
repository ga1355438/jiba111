using Microsoft.AspNetCore.Identity;
using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.Helpers;

public static class PasswordHelper
{
    private static readonly PasswordHasher<AdminUser> _hasher = new();

    public static string HashPassword(string password)
    {
        var admin = new AdminUser();
        return _hasher.HashPassword(admin, password);
    }

    public static bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var admin = new AdminUser { Password = hashedPassword };
        var result = _hasher.VerifyHashedPassword(admin, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success;
    }
}
