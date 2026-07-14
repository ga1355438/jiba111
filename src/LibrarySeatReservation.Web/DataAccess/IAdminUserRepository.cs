using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.DataAccess;

public interface IAdminUserRepository
{
    Task<AdminUser?> GetByUsernameAsync(string username);
}
