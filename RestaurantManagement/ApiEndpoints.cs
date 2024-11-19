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

    public static class Menu
    {
        private const string Base = $"{ApiBase}/menu";
        
        public const string Menus = Base;
        public const string ThisMenu = $"{Base}/{{id:guid}}";
        public const string UpdateMenu = $"{Base}/{{id:guid}}";
        public const string DeleteMenu = $"{Base}/{{id:guid}}";
    }
}