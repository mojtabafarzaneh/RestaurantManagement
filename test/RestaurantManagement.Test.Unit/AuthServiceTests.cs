using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using RestaurantManagement.Contracts.Requests;
using RestaurantManagement.Contracts.Responses;
using RestaurantManagement.Helper;
using RestaurantManagement.Models;
using RestaurantManagement.Repository;
using RestaurantManagement.Services;

namespace RestaurantManagement.Test.Unit;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

public class AuthServiceTests
{
    private readonly IAuthManager _authManager;
    private readonly ILogger<AuthService> _logger;
    private readonly RoleHelper _roleHelper;
    private readonly UserHelper _userHelper;
    private readonly AuthService _authService;
    private readonly TestUserFixture _testUserFixture;

    public AuthServiceTests()
    {
        _authManager = Substitute.For<IAuthManager>();
        _logger = Substitute.For<ILogger<AuthService>>();
        _testUserFixture = new TestUserFixture();

        var contextAccessor = _testUserFixture.CreateHttpContextAccessor(
            Guid.NewGuid().ToString(), 
            "admin@example.com", 
            "Admin"
        );
        _roleHelper = new RoleHelper(contextAccessor);
        _userHelper = new UserHelper(contextAccessor);

        _authService = new AuthService(_authManager, _logger, _roleHelper, _userHelper);
    }

    [Fact]
    public async Task Register_ShouldReturnErrors_WhenRegistrationFails()
    {
        // Arrange
        var request = new RegisterRequest { Email = "test@example.com", Password = "password" };
        var errors = new List<IdentityError>
        {
            new IdentityError { Description = "Email already exists" }
        };
        _authManager.Register(request).Returns(errors);

        // Act
        var action = async () => await _authService.Register(request);

        // Assert
        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Registration failed: Email already exists");
        await _authManager.Received(1).Register(request);
    }

    [Fact]
    public async Task Login_ShouldThrowUnauthorizedAccessException_WhenInvalidCredentials()
    {
        // Arrange
        var request = new LoginRequest { Email = "invalid@example.com", Password = "wrongPassword" };
        _authManager.DoesCustomerExist(request.Email).Returns((Customer)null!);

        // Act
        var action = async () => await _authService.Login(request);

        // Assert
        await action.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid username or password");
        await _authManager.Received(1).DoesCustomerExist(request.Email);
    }
    
    [Fact]
    public async Task VerifyRefreshToken_ShouldThrowException_WhenInvalidRefreshToken()
    {
        // Arrange
        var request = new AuthCustomerResponse { Token = "valid.token" };
        var handler = new JwtSecurityTokenHandler();
        var claims = new List<Claim> { new Claim("email", "test@example.com") };
        var identity = new ClaimsIdentity(claims);
        var token = handler.CreateJwtSecurityToken(subject: identity);
        request.Token = handler.WriteToken(token);

        var customer = new Customer { Id = new Guid(Guid.NewGuid().ToString()) };
        _authManager.DoesCustomerExist("test@example.com").Returns(customer);
        _authManager.DoesTokenExist(request, customer).Returns(true);

        // Act
        var action = async () => await _authService.VerifyRefreshToken(request);

        // Assert
        await action.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid refresh token");
        await _authManager.Received(1).DoesCustomerExist("test@example.com");
        await _authManager.Received(1).DoesTokenExist(request, customer);
    }


    [Fact]
    public async Task ChangeCustomerRole_ShouldThrowException_WhenUserIsNotAdmin()
    {
        // Arrange
        var contextAccessor = _testUserFixture.CreateHttpContextAccessor(
            Guid.NewGuid().ToString(), 
            "user@example.com", 
            "Customer"
        );
        var nonAdminRoleHelper = new RoleHelper(contextAccessor);
        var nonAdminAuthService = new AuthService(_authManager, _logger, nonAdminRoleHelper, _userHelper);

        var request = new ChangeUserRoleRequest { Id = new Guid(Guid.NewGuid().ToString()), Role = "Manager" };

        // Act
        var action = async () => await nonAdminAuthService.ChangeCustomerRole(request);

        // Assert
        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Me_ShouldReturnCustomerDetails_WhenValidUser()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var contextAccessor = _testUserFixture.CreateHttpContextAccessor(userId, "test@example.com", "Customer");
        var userHelper = new UserHelper(contextAccessor);

        var customer = new Customer { Id = new Guid(userId)};
        _authManager.DoesCustomerExist(userId).Returns(customer);
        _authManager.Me(customer).Returns(new CustomerResponse { Id = new Guid(userId), Email = "test@example.com" });

        var authService = new AuthService(_authManager, _logger, _roleHelper, userHelper);

        // Act
        var result = await authService.Me();

        // Assert
        result.Email.Should().Be("test@example.com");
        await _authManager.Received(1).DoesCustomerExist(userId);
    }
}
