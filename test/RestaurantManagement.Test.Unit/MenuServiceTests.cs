
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RestaurantManagement.Helper;
using RestaurantManagement.Models;
using RestaurantManagement.Repository;
using RestaurantManagement.Services;
using RestaurantManagement.Test.Unit;


namespace RestaurantManagement.Test.Unit;
public class MenuServiceTests
{
    private readonly ILogger<MenuService> _logger;
    private readonly IMenuManager _menuManager;
    private readonly RoleHelper _roleHelper;
    private readonly MenuService _menuService;
    private readonly TestUserFixture _testUserFixture;

    public MenuServiceTests()
    {
        _logger = Substitute.For<ILogger<MenuService>>();
        _menuManager = Substitute.For<IMenuManager>();
        _testUserFixture = new TestUserFixture();

        // Set up context accessor for a Chef role
        var contextAccessor = _testUserFixture.CreateHttpContextAccessor(
            Guid.NewGuid().ToString(), 
            "chef@example.com", 
            "Chef"
        );
        _roleHelper = new RoleHelper(contextAccessor);

        _menuService = new MenuService(_menuManager, _logger, _roleHelper);
    }

    [Fact]
    public async Task GetAllMenus_ShouldReturnMenus_WhenUserIsChef()
    {
        // Arrange
        var menus = new List<Menu>
        {
            new Menu { Id = Guid.NewGuid(), Name = "Pizza" },
            new Menu { Id = Guid.NewGuid(), Name = "Pasta" }
        };
        _menuManager.GetAllMenus().Returns(menus);

        // Act
        var result = await _menuService.GetAllMenus();

        // Assert
        result.Should().BeEquivalentTo(menus);
        await _menuManager.Received(1).GetAllMenus();
    }

    [Fact]
    public async Task GetAllMenus_ShouldThrowException_WhenUserIsNotChef()
    {
        // Arrange: Set up context accessor for a non-Chef role
        var contextAccessor = _testUserFixture.CreateHttpContextAccessor(
            Guid.NewGuid().ToString(), 
            "user@example.com", 
            "Customer"
        );
        var nonChefRoleHelper = new RoleHelper(contextAccessor);
        var nonChefMenuService = new MenuService(_menuManager, _logger, nonChefRoleHelper);

        // Act
        var action = async () => await nonChefMenuService.GetAllMenus();

        // Assert
        await action.Should().ThrowAsync<Exception>().WithMessage("You Are not Permitted to use this route!");
    }

    [Fact]
    public async Task CreateMenu_ShouldSetAvailablePropertyAndReturnCreatedMenu_WhenValidRequest()
    {
        // Arrange
        var request = new Menu { Name = "Burger", QuantityAvailable = 0 };
        var createdMenu = new Menu { Name = "Burger", QuantityAvailable = 0, Available = false };
        _menuManager.CreateMenu(request).Returns(createdMenu);

        // Act
        var result = await _menuService.CreateMenu(request);

        // Assert
        result.Should().BeEquivalentTo(createdMenu);
        await _menuManager.Received(1).CreateMenu(request);
    }

    [Fact]
    public async Task CreateMenu_ShouldThrowUnauthorizedAccessException_WhenUserIsNotChef()
    {
        // Arrange: Set up context accessor for a non-Chef role
        var contextAccessor = _testUserFixture.CreateHttpContextAccessor(
            Guid.NewGuid().ToString(), 
            "user@example.com", 
            "Customer"
        );
        var nonChefRoleHelper = new RoleHelper(contextAccessor);
        var nonChefMenuService = new MenuService(_menuManager, _logger, nonChefRoleHelper);

        // Act
        var action = async () => await nonChefMenuService.CreateMenu(new Menu());

        // Assert
        await action.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("False is not allowed to use this route!");
    }

    [Fact]
    public async Task GetMenuById_ShouldReturnMenu_WhenMenuExists()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var menu = new Menu { Id = menuId, Name = "Pasta" };
        _menuManager.GetMenuById(menuId).Returns(menu);

        // Act
        var result = await _menuService.GetMenuById(menuId);

        // Assert
        result.Should().BeEquivalentTo(menu);
        await _menuManager.Received(1).GetMenuById(menuId);
    }

    [Fact]
    public async Task GetMenuById_ShouldThrowException_WhenMenuIdIsEmpty()
    {
        // Act
        var action = async () => await _menuService.GetMenuById(Guid.Empty);

        // Assert
        await action.Should().ThrowAsync<Exception>().WithMessage("Menu could not be found!");
    }

    [Fact]
    public async Task DeleteMenuById_ShouldCallDeleteMenu_WhenMenuExists()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var menu = new Menu { Id = menuId, Name = "Pizza" };
        _menuManager.GetMenuById(menuId).Returns(menu);

        // Act
        await _menuService.DeleteMenuById(menuId);

        // Assert
        await _menuManager.Received(1).DeleteMenuById(menu);
    }

    [Fact]
    public async Task UpdateMenuById_ShouldCallUpdateMenu_WhenMenuExists()
    {
        // Arrange
        var menu = new Menu { Id = Guid.NewGuid(), Name = "Sushi" };
        _menuManager.IsExist(menu.Id).Returns(true);

        // Act
        await _menuService.UpdateMenuById(menu);

        // Assert
        await _menuManager.Received(1).UpdateMenuById(menu);
    }

    [Fact]
    public async Task UpdateMenuById_ShouldThrowException_WhenMenuDoesNotExist()
    {
        // Arrange
        var menu = new Menu { Id = Guid.NewGuid() };
        _menuManager.IsExist(menu.Id).Returns(false);

        // Act
        var action = async () => await _menuService.UpdateMenuById(menu);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>().WithMessage("Menu not found");
    }
}
