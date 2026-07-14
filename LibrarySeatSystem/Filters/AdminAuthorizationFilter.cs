using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LibrarySeatSystem.Filters;

public class AdminAuthorizationFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata
            .Any(e => e is Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute);

        if (allowAnonymous) return;

        var session = context.HttpContext.Session;
        var adminUsername = session.GetString("AdminUsername");

        if (string.IsNullOrEmpty(adminUsername))
        {
            context.Result = new RedirectToActionResult("Login", "Admin", null);
        }
    }
}
