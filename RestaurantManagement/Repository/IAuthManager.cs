using Microsoft.AspNetCore.Identity;
using RestaurantManagement.Contracts.Requests;

namespace RestaurantManagement.Repository;

public interface IAuthManager
{
    Task<IEnumerable<IdentityError>> Register(RegisterRequest request);
}