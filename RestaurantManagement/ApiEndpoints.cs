namespace RestaurantManagement;

public class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class Registration
    {
        private const string Base = $"{ApiBase}/registration";

        public const string Signup = Base;
        public const string Profile = $"{Base}/me";
        public const string Login = $"{Base}/login";
        public const string AdminRegister = $"{Base}/admin/register";
        public const string RefreshToken = $"{Base}/refresh";
    }
}