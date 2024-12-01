using RestaurantManagement.Models;
using RestaurantManagement.Repository;

namespace RestaurantManagement.Services;

public class MenuService: IMenuService
{
    private readonly IMenuManager _menuManager;
    private readonly ILogger<MenuService> _logger;

    public MenuService(IMenuManager menuManager, ILogger<MenuService> logger)
    {
        _menuManager = menuManager;
        _logger = logger;
    }

    public Task<List<Menu>> GetAllMenus()
    {
        return _menuManager.GetAllMenus();
    }

    public async Task<Menu> CreateMenu(Menu request)
    {
        
        var created = await _menuManager.CreateMenu(request);
        if (created == null)
        { 
            throw new ApplicationException("Failed to create menu");
        }

        return created;
    }

    public Task<Menu> GetMenuById(Guid id)
    {
        return _menuManager.GetMenuById(id);
    }

    public async Task DeleteMenuById(Guid id)
    {
        await _menuManager.DeleteMenuById(id);
    }

    public async Task UpdateMenuById(Menu request)
    {
        var doesExist = await _menuManager.IsExist(request.Id);
        if (doesExist == false)
        {
            throw new ArgumentException("Menu not found");
        }

        await _menuManager.UpdateMenuById(request);
    }
}