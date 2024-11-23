using System.Collections;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Contracts.Requests;
using RestaurantManagement.Contracts.Responses;
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
            return null;
        }

        if (await _context.Menus.AnyAsync(x => x.Name == request.Name))
        {
            throw new Exception($"Menu {request.Name} already exists!");
        }

        if (create.Entity.QuantityAvailable.Value == 0)
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

    public async Task<Menu?> GetMenuById(Guid Id)
    {
        var isChef = _roleService.IsUserChef();
        if (!isChef)
        {
            throw new Exception("You Are not Permitted to use this route!");
        }

        if (Id == null)
        {
            return null;
        }
        
        var result =await _context.Menus.FindAsync(Id);
        if (result is null)
        {
            return null;
        }
        
        return result;
    }

    public async Task DeleteMenuById(Guid Id)
    {
        var isChef = _roleService.IsUserChef();
        if (!isChef)
        {
            throw new Exception("You Are not Permitted to use this route!");
        }
        var isExist = await GetMenuById(Id);
        if (isExist is null)
        {
            throw new Exception($"Menu {Id} not found!");
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

    public async Task<bool> IsExist(Guid Id)
    {
        var entity = await GetMenuById(Id);
        return entity != null;
    }
}