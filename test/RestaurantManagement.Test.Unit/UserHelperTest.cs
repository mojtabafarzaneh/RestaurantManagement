using RestaurantManagement.Helper;

namespace RestaurantManagement.Test.Unit;

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

public class UserHelperTests
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly UserHelper _userHelper;
    
    private readonly TestUserFixture _fixture;

    public UserHelperTests()
    {
        _contextAccessor = Substitute.For<IHttpContextAccessor>();
        _userHelper = new UserHelper(_contextAccessor);
        
        _fixture = new TestUserFixture();

    }

    [Fact]
    public void UserId_ShouldReturnCorrectUserId_WhenUserIsAuthenticated()
    {
        // Arrange
        var expectedUserId = Guid.NewGuid().ToString();
        var context = _fixture.CreateHttpContextAccessor(
            expectedUserId, "expected@mail.com", "Customer");
        var userHelper = new UserHelper(context);
        
        // Act
        var result = userHelper.UserId();

        // Assert
        result.Should().Be(expectedUserId);
    }

    [Fact]
    public void UserId_ShouldReturnNull_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var context = _fixture.CreateHttpContextAccessor("", " ", "");
        var userHelper = new UserHelper(context);

        // Act
        var result = userHelper.UserId();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void UserId_ShouldReturnNull_WhenNoUidClaimExists()
    {
        // Arrange
        var context = _fixture.CreateHttpContextAccessor("", "email@email.com", "Customer");
        var userHelper = new UserHelper(context);

        // Act
        var result = userHelper.UserId();

        // Assert
        result.Should().BeNull();
    }
}
