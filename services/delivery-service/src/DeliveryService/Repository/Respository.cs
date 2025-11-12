using System.Data;
using Npgsql;
using SharedEvents.Models;

namespace DeliveryService.Repository{
  public class OrderWithDetails
  {
    public Order Order { get; set; } = new Order();
    public Restaurant Restaurant { get; set; } = new Restaurant();
    public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
  }

  public class Repository{
    private readonly string _connectionString;
    public Repository(string connectionString){
      _connectionString = connectionString;
    }

    public async Task<List<OrderWithDetails>> GetAllOrdersWithDetailsAsync()
    {
      var ordersWithDetails = new List<OrderWithDetails>();
      
      using var connection = new NpgsqlConnection(_connectionString);
      await connection.OpenAsync();

      // First, fetch all orders
      var ordersSql = @"
        SELECT OrderId, Status, Amount, RestaurantId, CreatedAt
        FROM Orders
        ORDER BY CreatedAt DESC;";

      var orders = new List<Order>();
      using (var command = new NpgsqlCommand(ordersSql, connection))
      using (var reader = await command.ExecuteReaderAsync())
      {
        while (await reader.ReadAsync())
        {
          orders.Add(new Order
          {
            OrderId = reader.GetInt32("OrderId"),
            Status = reader.GetString("Status"),
            Amount = reader.GetInt32("Amount"),
            RestaurantId = reader.GetInt32("RestaurantId"),
            CreatedAt = reader.GetDateTime("CreatedAt")
          });
        }
      }

      // For each order, fetch restaurant and menu items
      foreach (var order in orders)
      {
        var orderWithDetails = new OrderWithDetails { Order = order };

        // Fetch restaurant details
        var restaurantSql = @"
          SELECT Id, Name, Cuisine, Rating, Image, Eta, Location, CreatedAt
          FROM Restaurants
          WHERE Id = @RestaurantId;";

        using (var command = new NpgsqlCommand(restaurantSql, connection))
        {
          command.Parameters.AddWithValue("@RestaurantId", order.RestaurantId);
          using var reader = await command.ExecuteReaderAsync();
          
          if (await reader.ReadAsync())
          {
            orderWithDetails.Restaurant = new Restaurant
            {
              Id = reader.GetInt32("Id"),
              Name = reader.GetString("Name"),
              Cuisine = reader.GetString("Cuisine"),
              Rating = reader.GetDecimal("Rating"),
              Image = reader.IsDBNull("Image") ? string.Empty : reader.GetString("Image"),
              Eta = reader.GetInt32("Eta"),
              Location = reader.GetString("Location"),
              CreatedAt = reader.GetDateTime("CreatedAt")
            };
          }
        }

        // Fetch menu items for this order
        var menuItemsSql = @"
          SELECT m.Id, m.Name, m.Description, m.Price, m.Image, m.Veg, m.CreatedAt
          FROM MenuItems m
          INNER JOIN OrderMenuMappings omm ON m.Id = omm.MenuId
          WHERE omm.OrderId = @OrderId;";

        using (var command = new NpgsqlCommand(menuItemsSql, connection))
        {
          command.Parameters.AddWithValue("@OrderId", order.OrderId);
          using var reader = await command.ExecuteReaderAsync();
          
          while (await reader.ReadAsync())
          {
            orderWithDetails.MenuItems.Add(new MenuItem
            {
              Id = reader.GetInt32("Id"),
              Name = reader.GetString("Name"),
              Description = reader.IsDBNull("Description") ? string.Empty : reader.GetString("Description"),
              Price = reader.GetDecimal("Price"),
              Image = reader.IsDBNull("Image") ? string.Empty : reader.GetString("Image"),
              Veg = reader.GetBoolean("Veg"),
              CreatedAt = reader.GetDateTime("CreatedAt")
            });
          }
        }

        ordersWithDetails.Add(orderWithDetails);
      }

      return ordersWithDetails;
    }
  }
}