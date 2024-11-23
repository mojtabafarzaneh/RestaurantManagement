
using RestaurantManagement.Models;

namespace RestaurantManagement.Repository;

public interface IMenuManager
{
    public Task<List<Menu>> GetAllMenus();
    public Task<Menu> CreateMenu(Menu request);
    public Task<Menu> GetMenuById(Guid Id);
    
    public Task DeleteMenuById(Guid Id);
    public Task UpdateMenuById(Menu request);

    public Task<bool> IsExist(Guid Id);
}