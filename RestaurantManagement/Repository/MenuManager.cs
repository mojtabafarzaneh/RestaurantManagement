using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Services;

namespace RestaurantManagement.Repository;

public class MenuManager: IMenuManager
{
    private readonly ApplicationDBContex _context;
    private readonly RoleService _roleService;
    
    public MenuManager(ApplicationDBContex context, RoleService roleService)
    {
        _context = context;
        _roleService = roleService;
    }

    public async Task<List<Menu>> GetAllMenus()
    {
        var isChef = _roleService.IsUserChef();
        if (!isChef)
        {
            throw new Exception("You Are not Permitted to use this route!");
        }

        var result = await _context.Menus.ToListAsync();
        
        return result;
    }

    public async Task<Menu> CreateMenu(Menu request)
    {
        var isChef = _roleService.IsUserChef();
        if (!isChef)
        {
            throw new Exception("You Are not Permitted to use this route!");
        }
        
        var create = await _context.Menus.AddAsync(request);
        if (create is null || create.State == EntityState.Detached)
        {
            throw new Exception("Menu could not be created!");
        }

        if (await _context.Menus.AnyAsync(x => x.Name == request.Name))
        {
            throw new Exception($"Menu {request.Name} already exists!");
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
        var isChef = _roleService.IsUserChef();
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
            throw new Exception("There are no menus with this id in the system!");
        }
        
        return result;
    }

    public async Task DeleteMenuById(Guid id)
    {
        var isChef = _roleService.IsUserChef();
        if (!isChef)
        {
            throw new Exception("You Are not Permitted to use this route!");
        }
        var isExist = await GetMenuById(id);
        if (isExist is null)
        {
            throw new Exception($"Menu {id} not found!");
        }

        _context.Menus.Remove(isExist);
        await _context.SaveChangesAsync();

    }

    public async Task UpdateMenuById(Menu request)
    {
        var isChef = _roleService.IsUserChef();
        if (!isChef)
        {
            throw new Exception("You Are not Permitted to use this route!");
        }
        
        _context.Menus.Update(request);
        await _context.SaveChangesAsync();
        
    }

    public async Task<bool> IsExist(Guid id)
    {
        var entity = await GetMenuById(id);
        return entity != null;
    }
}