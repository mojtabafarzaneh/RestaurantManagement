using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using RestaurantManagement.Contracts.Requests;
using RestaurantManagement.Contracts.Responses;
using RestaurantManagement.Helper;
using RestaurantManagement.Models;
using RestaurantManagement.Repository;

namespace RestaurantManagement.Services;

public class AuthService: IAuthService
{
    private readonly IAuthManager _authManager;
    private readonly ILogger<AuthService> _logger;
    private readonly RoleHelper _roleHelper;
    private readonly UserHelper _userHelper;

    public AuthService(
        IAuthManager authManager, 
        ILogger<AuthService> logger, 
        RoleHelper roleHelper,
        UserHelper userHelper)
    {
        _authManager = authManager;
        _logger = logger;
        _roleHelper = roleHelper;
        _userHelper = userHelper;
    }

    public async Task<IEnumerable<IdentityError>> Register(RegisterRequest request)
    {
        try
        {
            var errors =  await _authManager.Register(request);
            var enumerable = errors as IdentityError[] ?? errors.ToArray();
            var identityErrors = errors as IdentityError[] ?? enumerable.ToArray();
            if (identityErrors.Any())
            {
                throw new ($"Registration failed: {identityErrors.First().Description}");
            }

            return enumerable;
        }
        catch (Exception)
        {
            _logger.LogError("Failed to register user");
            throw;
        }
        
    }

    public async Task<AuthCustomerResponse> Login(LoginRequest request)
    {
        try
        {
            var customer = await _authManager.DoesCustomerExist(request.Email);

            if (!await _authManager.IsPasswordValid(customer, request.Password))
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }
            return await _authManager.Login(customer);
        }
        catch (Exception)
        {
            _logger.LogError($"Failed to login");
            throw;
        }
    }

    public async Task<AuthCustomerResponse> VerifyRefreshToken(AuthCustomerResponse request)
    {
        try
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
            var email = tokenContent.Claims.ToList()
                .FirstOrDefault(x => x.Type == "email")?.Value;
            if (email == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }
            var customer = await _authManager.DoesCustomerExist(email);
            if (customer == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            if (await _authManager.DoesTokenExist(request, customer))
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }
                
            return await _authManager.VerifyRefreshToken(customer);
        }
        catch (Exception)
        {
            _logger.LogError($"Failed to verify refresh token");
            throw;
        }
    }

    public async Task ChangeCustomerRole(ChangeUserRoleRequest request)
    {
        try
        { 
            var authorizedUser = _roleHelper.IsAdminUser();
            if (!authorizedUser)
            {
                throw new UnauthorizedAccessException();
            }

            var customer = await _authManager.DoesCustomerExist(request.Id);

            if (customer is null)
            {
                throw new Exception("User not found");
            }

            if (!await _authManager.DoesRoleExist(request.Role))
            {
                throw new Exception("Role does not exist");
            }
            var currentRole = await _authManager.GetCustomerRoles(customer, request.Role);
            if (currentRole == null)
            {
                throw new ArgumentException("You are not Allowed to change the role of this user");
            }

            await _authManager.RemoveCurrentRole(customer, currentRole);
            await _authManager.ChangeCustomerRole(request, customer);
        }
        catch (Exception)
        {
            _logger.LogError($"Failed to change role");
            throw;
        }
    }

    public async Task<Customer> Me()
    {
        try
        {
            var userId = _userHelper.UserId();
            if (userId is null)
            {
                throw new UnauthorizedAccessException();
            }
            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new ArgumentException("Invalid UserId format.");
            }
            var customer = await _authManager.DoesCustomerExist(userGuid);
            if (customer is null)
            {
                throw new UnauthorizedAccessException();
            }
            
            return customer;
        }
        catch (Exception)
        {
            _logger.LogError($"Failed to get user");
            throw;
        }
    }
}