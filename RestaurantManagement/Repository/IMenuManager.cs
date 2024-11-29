
using RestaurantManagement.Models;

namespace RestaurantManagement.Repository;

public interface IMenuManager
{
    public Task<List<Menu>> GetAllMenus();
    public Task<Menu> CreateMenu(Menu request);
    public Task<Menu> GetMenuById(Guid id);
    
    public Task DeleteMenuById(Guid id);
    public Task UpdateMenuById(Menu request);

    public Task<bool> IsExist(Guid id);
}