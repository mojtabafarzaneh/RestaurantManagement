namespace RestaurantManagement.Contracts.Responses;

public class AuthCustomerResponse
{
    public Guid CustomerId { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}