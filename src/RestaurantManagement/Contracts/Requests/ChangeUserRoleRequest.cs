namespace RestaurantManagement.Contracts.Requests;

public class ChangeUserRoleRequest
{
    public Guid Id { get; set; }
    public string Role { get; set; }
}