namespace RestaurantManagement.Services;

public class UserService
{
    private readonly IHttpContextAccessor _ContextAccessor;

    public UserService(IHttpContextAccessor contextAccessor)
    {
        _ContextAccessor = contextAccessor;
    }

    public string UserId()
    {
        var user = _ContextAccessor.HttpContext.User;
        if (user == null || !user.Identity.IsAuthenticated)
        {
            return null;
        }
        var userId = user.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }
        return userId;
    }
}