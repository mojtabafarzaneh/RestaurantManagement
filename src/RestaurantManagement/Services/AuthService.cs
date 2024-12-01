using Microsoft.AspNetCore.Identity;
using RestaurantManagement.Contracts.Requests;
using RestaurantManagement.Contracts.Responses;
using RestaurantManagement.Repository;

namespace RestaurantManagement.Services;

public class AuthService: IAuthService
{
    private readonly IAuthManager _authManager;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IAuthManager authManager, ILogger<AuthService> logger)
    {
        _authManager = authManager;
        _logger = logger;
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
        catch (System.Exception)
        {
            _logger.LogError("Failed to register user");
            throw;
        }
        
    }

    public Task<AuthCustomerResponse> Login(LoginRequest request)
    {
        try
        {
            return _authManager.Login(request);
        }
        catch (System.Exception)
        {
            _logger.LogError($"Failed to login");
            throw;
        }
    }

    public Task<AuthCustomerResponse> VerifyRefreshToken(AuthCustomerResponse request)
    {
        try
        {
            return _authManager.VerifyRefreshToken(request);
        }
        catch (System.Exception)
        {
            _logger.LogError($"Failed to verify refresh token");
            throw;
        }
    }

    public Task ChangeCustomerRole(ChangeUserRoleRequest request)
    {
        try
        {
            return _authManager.ChangeCustomerRole(request);
        }
        catch (System.Exception)
        {
            _logger.LogError($"Failed to change role");
            throw;
        }
    }

    public Task<CustomerResponse> Me()
    {
        try
        {
            return _authManager.Me();
        }
        catch (System.Exception)
        {
            _logger.LogError($"Failed to get user");
            throw;
        }
    }
}