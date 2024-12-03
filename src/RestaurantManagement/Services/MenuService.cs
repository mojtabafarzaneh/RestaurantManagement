using RestaurantManagement.Helper;
using RestaurantManagement.Models;
using RestaurantManagement.Repository;

namespace RestaurantManagement.Services;

public class MenuService: IMenuService
{
    private readonly IMenuManager _menuManager;
    private readonly ILogger<MenuService> _logger;
    private readonly RoleHelper _roleHelper;

    public MenuService(IMenuManager menuManager, ILogger<MenuService> logger, RoleHelper roleHelper)
    {
        _menuManager = menuManager;
        _logger = logger;
        _roleHelper = roleHelper;
    }

    public Task<List<Menu>> GetAllMenus()
    {
        try
        {
            var isChef = _roleHelper.IsUserChef();
            if (!isChef)
            {
                throw new Exception("You Are not Permitted to use this route!");
            }

            return _menuManager.GetAllMenus();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<Menu> CreateMenu(Menu request)
    {
        try
        {
            var isChef = _roleHelper.IsUserChef();
            if (!isChef)
            {
                throw new UnauthorizedAccessException($"{isChef} is not allowed to use this route!");
            }
            if (request.QuantityAvailable != null && request.QuantityAvailable.Value == 0)
            {
                request.Available= false;
            }
            else
            {
                request.Available = true;
            }

            var created = await _menuManager.CreateMenu(request);
            if (created == null)
            {
                throw new ApplicationException("Failed to create menu");
            }

            return created;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
        
    }

    public async Task<Menu> GetMenuById(Guid id)
    {
        try
        {
            var isChef = _roleHelper.IsUserChef();
            if (!isChef)
            {
                throw new Exception("You Are not Permitted to use this route!");
            }

            if (id == Guid.Empty)
            {
                throw new Exception("Menu could not be found!");
            }
            return await _menuManager.GetMenuById(id);

        }
        catch (Exception e)
        { 
            _logger.LogError(e, e.Message);
            throw;
        } 
    }

    public async Task DeleteMenuById(Guid id)
    {
        try
        {
            var isChef = _roleHelper.IsUserChef();
            if (!isChef)
            {
                throw new UnauthorizedAccessException("You Are not Permitted to use this route!");
            }

            var menu = await GetMenuById(id);
            if (menu is null)
            {
                throw new ArgumentException($"Menu {id} not found!");
            }
            
            await _menuManager.DeleteMenuById(menu);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task UpdateMenuById(Menu request)
    {
        try
        {
            var isChef = _roleHelper.IsUserChef();
            if (!isChef)
            {
                throw new Exception("You Are not Permitted to use this route!");
            }
            var doesExist = await _menuManager.IsExist(request.Id);
            if (doesExist == false)
            {
                throw new ArgumentException("Menu not found");
            }

            await _menuManager.UpdateMenuById(request);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}