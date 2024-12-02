namespace RestaurantManagement.Test.Unit;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RestaurantManagement.Helper;
using RestaurantManagement.Models;
using RestaurantManagement.Repository;
using RestaurantManagement.Services;
using Xunit;

public class CardServiceTests
{
    private readonly ILogger<CardService> _logger;
    private readonly ICardManager _cardManager;
    private readonly RoleHelper _roleHelper;
    private readonly UserHelper _userHelper;
    private readonly CardService _service;
    private readonly TestUserFixture _testUserFixture;

    public CardServiceTests()
    {
        _logger = Substitute.For<ILogger<CardService>>();
        _cardManager = Substitute.For<ICardManager>();
        _testUserFixture = new TestUserFixture();

        var contextAccessor = _testUserFixture.CreateHttpContextAccessor(Guid.NewGuid().ToString(), "test@example.com", "Admin");
        _roleHelper = new RoleHelper(contextAccessor);
        _userHelper = new UserHelper(contextAccessor);

        _service = new CardService(_logger, _cardManager, _roleHelper, _userHelper);
    }

    [Fact]
    public async Task GetAllCardsAsync_ShouldThrowUnauthorizedAccessException_WhenUserHasNoValidRoles()
    {
        // Arrange: Set the user with no valid roles
        var contextAccessor = _testUserFixture.CreateHttpContextAccessor(Guid.NewGuid().ToString(), "test@example.com", "User");
        var roleHelper = new RoleHelper(contextAccessor);
        var userHelper = new UserHelper(contextAccessor);
        var service = new CardService(_logger, _cardManager, roleHelper, userHelper);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.GetAllCardsAsync());
    }

    [Fact]
    public async Task GetAllCardsAsync_ShouldReturnListOfCards_WhenUserIsAdmin()
    {
        // Arrange
        var expectedCards = new List<Card> { new Card { Id = Guid.NewGuid() } };
        _cardManager.GetAllCardsAsync().Returns(expectedCards);

        // Act
        var result = await _service.GetAllCardsAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedCards);
    }

    [Fact]
    public async Task GetCardByIdAsync_ShouldThrowUnauthorizedAccessException_WhenUserIdIsNull()
    {
        // Arrange: Set userId to null
        var contextAccessor = _testUserFixture.CreateHttpContextAccessor("", "test@example.com", "Admin");
        var userHelper = new UserHelper(contextAccessor);
        var service = new CardService(_logger, _cardManager, _roleHelper, userHelper);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.GetCardByIdAsync());
    }

    [Fact]
    public async Task GetCardByIdAsync_ShouldReturnCard_WhenUserIdIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var card = new Card { Id = Guid.NewGuid(), CustomerId = Guid.Parse(userId) };
        _cardManager.GetCardByIdAsync(Arg.Any<Guid>()).Returns(card);

        // Act
        var result = await _service.GetCardByIdAsync();

        // Assert
        result.Should().Be(card);
    }

    [Fact]
    public async Task CreateCardAsync_ShouldThrowArgumentException_WhenCustomerDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        _cardManager.DoesCustomerExists(Arg.Any<Guid>()).Returns(false);

        var request = new CardItem { MenuId = Guid.NewGuid(), Quantity = 1 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateCardAsync(request));
    }

    [Fact]
    public async Task CreateCardAsync_ShouldThrowNullReferenceException_WhenMenuDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        _cardManager.DoesCustomerExists(Arg.Any<Guid>()).Returns(true);
        _cardManager.DoesMenuExist(Arg.Any<Guid>()).Returns((Menu)null);

        var request = new CardItem { MenuId = Guid.NewGuid(), Quantity = 1 };

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => _service.CreateCardAsync(request));
    }

    [Fact]
    public async Task CreateCardAsync_ShouldSucceed_WhenAllConditionsAreMet()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var menu = new Menu { Id = Guid.NewGuid(), QuantityAvailable = 5 };
        _cardManager.DoesCustomerExists(Arg.Any<Guid>()).Returns(true);
        _cardManager.DoesMenuExist(Arg.Any<Guid>()).Returns(menu);

        var request = new CardItem { MenuId = menu.Id, Quantity = 3 };

        // Act
        await _service.CreateCardAsync(request);

        // Assert
        await _cardManager.Received(1).CreateCardAsync(Arg.Any<CardItem>(), Arg.Any<Guid>(), Arg.Any<Card>(), Arg.Is<Menu>(m => m == menu));
    }


}
