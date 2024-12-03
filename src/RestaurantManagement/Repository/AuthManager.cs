using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantManagement.Contracts.Requests;
using RestaurantManagement.Contracts.Responses;
using RestaurantManagement.Models;
using RestaurantManagement.Helper;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace RestaurantManagement.Repository;

public class AuthManager: IAuthManager
{
    private readonly UserManager<Customer> _userManager;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private readonly RoleManager<RestaurantRoles> _roleManager;
    private readonly RoleHelper _roleHelper;
    private readonly UserHelper _userHelper;
    private Customer _customer;

    public AuthManager(UserManager<Customer> userManager, IMapper mapper, IConfiguration config, RoleManager<RestaurantRoles> roleManager, RoleHelper roleHelper, UserHelper userHelper)
    {
        _config = config;
        _roleManager = roleManager;
        _roleHelper = roleHelper;
        _userHelper = userHelper;
        _userManager = userManager;
        _mapper = mapper;
    }
    public async Task<IEnumerable<IdentityError>> Register(RegisterRequest request)
    {
        var user = _mapper.Map<Customer>(request);
        user.UserName = request.Email;
        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Customer");
        }
        return result.Errors;
    }

    public async Task<Customer> DoesCustomerExist(string email)
    {
        var result = await _userManager.FindByEmailAsync(email);
        if (result == null)
        {
            return null!;
        }

        return result;
    }

    public async Task<bool> IsPasswordValid(Customer customer, string password)
    {
        return await _userManager.CheckPasswordAsync(customer, password);
    }

    public async Task<AuthCustomerResponse> Login(LoginRequest request)
    {
        var token = await GenerateJwtToken();
        return new AuthCustomerResponse
        {
            Token = token,
            CustomerId = _customer.Id,
            RefreshToken = await GenerateRefreshToken(),
        };
    }

    private async Task<string> GenerateJwtToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? string.Empty));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var roles = await _userManager.GetRolesAsync(_customer);
        var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
        var userClaims = await _userManager.GetClaimsAsync(_customer);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, _customer.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, _customer.Email),
            new Claim("uid", _customer.Id.ToString())
        }.Union(userClaims).Union(roleClaims);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToInt32(_config["Jwt:DurationInMinutes"])),
            signingCredentials: credentials
            );
        
        return new JwtSecurityTokenHandler().WriteToken(token);

    }

    private async Task<string> GenerateRefreshToken()
    {
        await _userManager.RemoveAuthenticationTokenAsync(
            _customer, "RestaurantManagement", "RefreshToken");
        
        var newRefreshToken = await _userManager.GenerateUserTokenAsync(
            _customer, "RestaurantManagementApi", "RefreshToken");
        var result = await _userManager.SetAuthenticationTokenAsync(
            _customer, "RestaurantManagementApi", "RefreshToken", newRefreshToken);

        return newRefreshToken;
    }

    public async Task<bool> DoesTokenExist(AuthCustomerResponse request, Customer customer)
    {
        return await  _userManager.VerifyUserTokenAsync(
            customer, "RestaurantManagementApi", 
            "RefreshToken", request.RefreshToken);
    }
    

    public async Task<AuthCustomerResponse> VerifyRefreshToken(AuthCustomerResponse request)
    {
        var token = await GenerateJwtToken();
        return new AuthCustomerResponse
        {
            Token = token,
            CustomerId = _customer.Id,
            RefreshToken = await GenerateRefreshToken(),
        };
    }

    public async Task<Customer?> DoesCustomerExist(Guid id)
    {
        var customer = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);

        if (customer == null)
        {
            return null;
        }
        return customer;

    }

    public async Task<bool> DoesRoleExist(string role)
    {
        return await _roleManager.RoleExistsAsync(role);
    }

    public async Task<IList<string>?> GetCustomerRoles(Customer customer, string role )
    {
        var currentRole = await _userManager.GetRolesAsync(customer);
        if (!currentRole.Contains(role) ||
            currentRole.Contains("Admin") ||
            currentRole.Contains("Chef") ||
            currentRole.Contains("Manager"))
        {
            return null;
        }
        return currentRole;
    }

    public async Task RemoveCurrentRole(Customer customer, IEnumerable<string> role)
    {
        try
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(customer, role);
            if (!removeResult.Succeeded)
            {
                throw new Exception(
                    $"Failed to remove roles: " +
                    $"{string.Join(", ", removeResult.Errors.Select(e => e.Description))}");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
        
    }

    public async Task ChangeCustomerRole(ChangeUserRoleRequest request, Customer customer)
    {

        var addResult = await _userManager.AddToRoleAsync(customer, request.Role);
        if (!addResult.Succeeded)
        {
            throw new ArgumentException("Failed to add users new role");
        }
    }

    public async Task<CustomerResponse> Me(Customer customer)
    {
        var resultCustomer = _mapper.Map<CustomerResponse>(customer);

        return resultCustomer;

    }
}