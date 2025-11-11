using OrderService.Models;

namespace OrderService.Data
{
    public interface IOrderRepository
    {
        Task<int> CreateRestaurantAsync(Restaurant restaurant);
        Task<int> CreateMenuItemAsync(MenuItem menuItem);
        Task CreateRestaurantMenuMappingAsync(int restaurantId, int menuId);
        Task<int> AddRestaurantWithMenuItemsAsync(Restaurant restaurant, List<MenuItem> menuItems);
        Task InitializeDatabaseAsync();
    }
}

