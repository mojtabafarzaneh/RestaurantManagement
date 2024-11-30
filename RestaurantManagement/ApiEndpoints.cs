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

    public static class Card
    {
        private const string Base = $"{ApiBase}/card";
        public const string AllCards = $"{Base}";
        public const string DeleteCard = $"{Base}/me";
        public const string MyCard = $"{Base}/me";
        public const string CreateCard = $"{Base}";
        public const string CardItems = $"{Base}/me/items";
        public const string CardItemsById = $"{Base}/me/items/{{id:guid}}";
        public const string UpdateCardItems = $"{Base}/me/items/{{id:guid}}";
        public const string DeleteCardItems = $"{Base}/me/items/{{id:guid}}";
    }

    public static class Order
    {
        private const string Base = $"{ApiBase}/order";
        public const string CreateOrder = $"{Base}";
        public const string UpdateOrder = $"{Base}/{{id:guid}}";
        public const string DeleteOrder = $"{Base}/me";
        public const string GetOrder = $"{Base}/me";
        public const string GetAllOrder = $"{Base}";
        public const string GetTicket = $"{Base}/ticket/";
        
    }
}