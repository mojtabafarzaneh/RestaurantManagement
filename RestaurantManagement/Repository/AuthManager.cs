using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RestaurantManagement.Contracts.Requests;
using RestaurantManagement.Contracts.Responses;
using RestaurantManagement.Models;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace RestaurantManagement.Repository;

public class AuthManager: IAuthManager
{
    private readonly UserManager<Customer> _userManager;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private Customer _customer;

    public AuthManager(UserManager<Customer> userManager, IMapper mapper, IConfiguration config)
    {
        this._config = config;
        this._userManager = userManager;
        this._mapper = mapper;
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

    public async Task<AuthCustomerResponse> Login(LoginRequest request)
    {
        bool isValid = false;
        _customer = await _userManager.FindByEmailAsync(request.Email);
        isValid = await _userManager.CheckPasswordAsync(_customer, request.Password);
        if (_customer is null || !isValid)
        {
            return null;
        }

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
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
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

    public async Task<AuthCustomerResponse> VerifyRefreshToken(AuthCustomerResponse request)
    {
        var JwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var tokenContent = JwtSecurityTokenHandler.ReadJwtToken(request.Token);
        var email = tokenContent.Claims.ToList()
            .FirstOrDefault(x => x.Type == "email")?.Value;
        _customer = await _userManager.FindByEmailAsync(email);

        if (_customer is null)
        {
            return null;
        }
        var isValidRefreshToken = await  _userManager.VerifyUserTokenAsync(
            _customer, "RestaurantManagementApi", 
            "RefreshToken", request.RefreshToken);
        if (!isValidRefreshToken)
        {
            await _userManager.UpdateSecurityStampAsync(_customer);
            return null;
        }
        var token = await GenerateJwtToken();
        return new AuthCustomerResponse
        {
            Token = token,
            CustomerId = _customer.Id,
            RefreshToken = await GenerateRefreshToken(),
        };

    }
}