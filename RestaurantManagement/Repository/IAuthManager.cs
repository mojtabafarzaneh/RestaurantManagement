using System.Collections;
using Microsoft.AspNetCore.Identity;
using RestaurantManagement.Contracts.Requests;
using RestaurantManagement.Contracts.Responses;

namespace RestaurantManagement.Repository;

public interface IAuthManager
{
    Task<IEnumerable<IdentityError>> Register(RegisterRequest request);
    Task<AuthCustomerResponse> Login(LoginRequest request);
    Task<AuthCustomerResponse> VerifyRefreshToken(AuthCustomerResponse request);
    Task ChangeCustomerRole(ChangeUserRoleRequest request);

    Task<CustomerResponse> Me();
}