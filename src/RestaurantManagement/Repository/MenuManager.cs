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
        var result = await _context.Menus.ToListAsync();
        
        return result;
    }

    public async Task<bool> DoesMenuExist(string name)
    {
        return await _context.Menus.AnyAsync(x => x.Name == name);
        
    }

    public async Task<Menu> CreateMenu(Menu request)
    {
        var create = await _context.Menus.AddAsync(request);
        if (create is null || create.State == EntityState.Detached)
        {
            throw new ArgumentException("Menu could not be created!");
        }

        if (await _context.Menus.AnyAsync(x => x.Name == request.Name))
        {
            throw new ArgumentException("the menu already exists!");
        }
        
        await _context.SaveChangesAsync();
        return create.Entity;
    }

    public async Task<Menu> GetMenuById(Guid id)
    {
        var result =await _context.Menus.Where (x => x.Id == id).FirstOrDefaultAsync();
        if (result is null)
        {
            throw new ArgumentException("the menu does not exist!");
        }
        
        return result;
    }

    public async Task DeleteMenuById(Menu menu)
    {
        try
        {
            _context.Menus.Remove(menu);
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