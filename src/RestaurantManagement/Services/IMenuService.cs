using RestaurantManagement.Models;

namespace RestaurantManagement.Services;

public interface IMenuService
{
    public Task<List<Menu>> GetAllMenus();
    public Task<Menu> CreateMenu(Menu request);
    public Task<Menu> GetMenuById(Guid id);
    
    public Task DeleteMenuById(Guid id);
    public Task UpdateMenuById(Menu request);
    
}