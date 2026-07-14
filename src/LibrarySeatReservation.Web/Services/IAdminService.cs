using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.Services;

public interface IAdminService
{
    Task<AdminUser?> ValidateLoginAsync(string username, string password);
}
