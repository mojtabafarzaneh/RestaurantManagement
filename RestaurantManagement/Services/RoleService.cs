using System.Security.Claims;

namespace RestaurantManagement.Services;

public class RoleService
{
    private readonly IHttpContextAccessor _ContextAccessor;

    public RoleService(IHttpContextAccessor contextAccessor)
    {
        _ContextAccessor = contextAccessor;
    }

    public bool IsAdminUser()
    {
        var user = _ContextAccessor.HttpContext?.User;
        if (user is null || !user.Identity.IsAuthenticated)
        {
            return false;
        }
        return user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
    }

    public bool IsUserChef()
    {
        var user = _ContextAccessor.HttpContext?.User;
        if (user is null || !user.Identity.IsAuthenticated)
        {
            return false;
        }
        return user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Chef");
    }

    public bool IsManagerUser()
    {
        var user = _ContextAccessor.HttpContext?.User;
        if (user is null || !user.Identity.IsAuthenticated)
        {
            return false;
        }
        return user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Manager");
        
    }
}