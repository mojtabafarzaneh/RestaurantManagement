using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Helper;

namespace RestaurantManagement.Repository;

public class MenuManager: IMenuManager
{
    private readonly ApplicationDBContex _context;
    private readonly RoleHelper _roleHelper;
    private readonly ILogger<MenuManager> _logger;
    
    public MenuManager(ApplicationDBContex context, RoleHelper roleHelper, ILogger<MenuManager> logger)
    {
        _context = context;
        _roleHelper = roleHelper;
        _logger = logger;
    }

    public async Task<List<Menu>> GetAllMenus()
    {
        var isChef = _roleHelper.IsUserChef();
        if (!isChef)
        {
            throw new Exception("You Are not Permitted to use this route!");
        }

        var result = await _context.Menus.ToListAsync();
        
        return result;
    }

    public async Task<Menu> CreateMenu(Menu request)
    {
        var isChef = _roleHelper.IsUserChef();
        if (!isChef)
        {
            throw new UnauthorizedAccessException($"{isChef} is not allowed to use this route!");
        }
        
        var create = await _context.Menus.AddAsync(request);
        if (create is null || create.State == EntityState.Detached)
        {
            throw new ArgumentException("Menu could not be created!");
        }

        if (await _context.Menus.AnyAsync(x => x.Name == request.Name))
        {
            throw new ArgumentException("the menu already exists!");
        }

        if (create.Entity.QuantityAvailable != null && create.Entity.QuantityAvailable.Value == 0)
        {
            create.Entity.Available = false;
        }
        else
        {
            create.Entity.Available = true;
        }
        await _context.SaveChangesAsync();
        return create.Entity;
    }

    public async Task<Menu> GetMenuById(Guid id)
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
        
        var result =await _context.Menus.FindAsync(id);
        if (result is null)
        {
            throw new Exception();
        }
        
        return result;
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

            var isExist = await GetMenuById(id);
            if (isExist is null)
            {
                throw new ArgumentException($"Menu {id} not found!");
            }

            _context.Menus.Remove(isExist);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);   
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

            _context.Menus.Update(request);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
        
    }

    public async Task<bool> IsExist(Guid id)
    {
        var entity = await GetMenuById(id);
        return entity != null;
    }
}