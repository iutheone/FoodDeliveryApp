using SharedEvents.Models;

namespace OrderService.Data
{
    public interface IOrderRepository
    {
        Task<int> CreateRestaurantAsync(Restaurant restaurant);
        Task<int> CreateMenuItemAsync(MenuItem menuItem);
        Task CreateRestaurantMenuMappingAsync(int restaurantId, int menuId);
        Task<int> AddRestaurantWithMenuItemsAsync(Restaurant restaurant, List<MenuItem> menuItems);
        Task InitializeDatabaseAsync();
        Task<List<Restaurant>> GetAllRestaurents();
        Task<List<MenuItem>> GetMenuItems(int restaurantID);
        Task<int> PlaceOrderRequest(Order order, List<int> mapping);
    }
}

