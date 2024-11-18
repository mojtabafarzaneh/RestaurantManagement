namespace RestaurantManagement.Contracts.Requests;

public class ChangeUserRoleRequest
{
    public string Id { get; set; }
    public string Role { get; set; }
}