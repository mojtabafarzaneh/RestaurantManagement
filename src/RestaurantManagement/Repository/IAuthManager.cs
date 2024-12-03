using System.Collections;
using Microsoft.AspNetCore.Identity;
using RestaurantManagement.Contracts.Requests;
using RestaurantManagement.Contracts.Responses;
using RestaurantManagement.Models;

namespace RestaurantManagement.Repository;

public interface IAuthManager
{
    Task<IEnumerable<IdentityError>> Register(RegisterRequest request);
    Task<AuthCustomerResponse> Login(LoginRequest request);
    Task<AuthCustomerResponse> VerifyRefreshToken(AuthCustomerResponse request);
    Task ChangeCustomerRole(ChangeUserRoleRequest request, Customer customer);
    public Task<IList<string>?> GetCustomerRoles(Customer customer, string role);
    public Task RemoveCurrentRole(Customer customer, IEnumerable<string> role);
    public Task<bool> DoesRoleExist(string role);
    public Task<Customer> DoesCustomerExist(string email);
    public Task<bool> IsPasswordValid(Customer customer, string password);

    public Task<Customer?> DoesCustomerExist(Guid id);

    public Task<bool> DoesTokenExist(AuthCustomerResponse request, Customer customer);

    Task<CustomerResponse> Me(Customer customer);
}