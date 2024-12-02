using RestaurantManagement.Helper;

namespace RestaurantManagement.Test.Unit;

using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

public class RoleHelperTests
{
    private readonly TestUserFixture _fixture;

    private readonly IHttpContextAccessor _contextAccessor;
    private readonly RoleHelper _roleHelper;

    public RoleHelperTests()
    {
        _contextAccessor = Substitute.For<IHttpContextAccessor>();
        _roleHelper = new RoleHelper(_contextAccessor);
        
        _fixture = new TestUserFixture();

    }

    [Fact]
    public void IsAdminUser_ShouldReturnTrue_WhenUserIsAdmin()
    {
        // Arrange
        
        var context = _fixture.CreateHttpContextAccessor(Guid.NewGuid().ToString(), "admin@example.com", "Admin");
        var roleHelper = new RoleHelper(context);
        // Act
        var result = roleHelper.IsAdminUser();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsAdminUser_ShouldReturnFalse_WhenUserIsNotAdmin()
    {
        // Arrange
        var context = _fixture.CreateHttpContextAccessor(Guid.NewGuid().ToString(), "example@example.com", "Customer");
        var roleHelper = new RoleHelper(context);

        // Act
        var result = roleHelper.IsAdminUser();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsAdminUser_ShouldReturnFalse_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var context = _fixture.CreateHttpContextAccessor("", "", "");
        var roleHelper = new RoleHelper(context);
        
        

        // Act
        var result = roleHelper.IsAdminUser();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsUserChef_ShouldReturnTrue_WhenUserIsChef()
    {
        // Arrange
        var context = _fixture.CreateHttpContextAccessor(Guid.NewGuid().ToString(), "chef@example.com", "Chef");
        var roleHelper = new RoleHelper(context);
        // Act
        var result = roleHelper.IsUserChef();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsManagerUser_ShouldReturnTrue_WhenUserIsManager()
    {
        // Arrange
        var context = _fixture.CreateHttpContextAccessor(Guid.NewGuid().ToString(), "manager@example.com", "Manager");
        var roleHelper = new RoleHelper(context);

        // Act
        var result = roleHelper.IsManagerUser();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void RoleHelper_ShouldReturnFalse_WhenHttpContextIsNull()
    {
        // Arrange
        _contextAccessor.HttpContext.Returns((HttpContext)null);

        // Act
        var isAdmin = _roleHelper.IsAdminUser();
        var isChef = _roleHelper.IsUserChef();
        var isManager = _roleHelper.IsManagerUser();

        // Assert
        isAdmin.Should().BeFalse();
        isChef.Should().BeFalse();
        isManager.Should().BeFalse();
    }
}
