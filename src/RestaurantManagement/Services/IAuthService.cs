using Microsoft.AspNetCore.Identity;
using RestaurantManagement.Contracts.Requests;
using RestaurantManagement.Contracts.Responses;
using RestaurantManagement.Models;

namespace RestaurantManagement.Services;

public interface IAuthService
{
    Task<IEnumerable<IdentityError>> Register(RegisterRequest request);
    Task<AuthCustomerResponse> Login(LoginRequest request);
    Task<AuthCustomerResponse> VerifyRefreshToken(AuthCustomerResponse request);
    Task ChangeCustomerRole(ChangeUserRoleRequest request);

    Task<Customer> Me();
}