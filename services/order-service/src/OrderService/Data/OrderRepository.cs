using System.Data;
using Npgsql;
using SharedEvents.Models;

namespace OrderService.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task InitializeDatabaseAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // Create Restaurants table
            var createRestaurantsTable = @"
                CREATE TABLE IF NOT EXISTS Restaurants (
                    Id SERIAL PRIMARY KEY,
                    Name VARCHAR(255) NOT NULL,
                    Cuisine VARCHAR(100) NOT NULL,
                    Rating DECIMAL(3,2) NOT NULL,
                    Image VARCHAR(500),
                    Eta INTEGER NOT NULL,
                    Location VARCHAR(255) NOT NULL,
                    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
                );";

            // Create MenuItems table
            var createMenuItemsTable = @"
                CREATE TABLE IF NOT EXISTS MenuItems (
                    Id SERIAL PRIMARY KEY,
                    Name VARCHAR(255) NOT NULL,
                    Description TEXT,
                    Price DECIMAL(10,2) NOT NULL,
                    Image VARCHAR(500),
                    Veg BOOLEAN NOT NULL,
                    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
                );";

            // Create RestaurantMenuMappings table
            var createRestaurantMenuMappingsTable = @"
                CREATE TABLE IF NOT EXISTS RestaurantMenuMappings (
                    Id SERIAL PRIMARY KEY,
                    RestaurantId INTEGER NOT NULL REFERENCES Restaurants(Id) ON DELETE CASCADE,
                    MenuId INTEGER NOT NULL REFERENCES MenuItems(Id) ON DELETE CASCADE,
                    UNIQUE(RestaurantId, MenuId)
                );";

            // Create Orders table
            var createOrdersTable = @"
                CREATE TABLE IF NOT EXISTS Orders (
                    OrderId SERIAL PRIMARY KEY,
                    Status VARCHAR(50) NOT NULL,
                    Amount integer not null,
                    RestaurantId INTEGER NOT NULL REFERENCES Restaurants(Id),
                    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
                );";

            // Create OrderMenuMappings table
            var createOrderMenuMappingsTable = @"
                CREATE TABLE IF NOT EXISTS OrderMenuMappings (
                    Id SERIAL PRIMARY KEY,
                    OrderId INTEGER NOT NULL REFERENCES Orders(OrderId) ON DELETE CASCADE,
                    MenuId INTEGER NOT NULL REFERENCES MenuItems(Id) ON DELETE CASCADE
                );";

            using var command = connection.CreateCommand();
            command.CommandText = createRestaurantsTable;
            await command.ExecuteNonQueryAsync();

            command.CommandText = createMenuItemsTable;
            await command.ExecuteNonQueryAsync();

            command.CommandText = createRestaurantMenuMappingsTable;
            await command.ExecuteNonQueryAsync();

            command.CommandText = createOrdersTable;
            await command.ExecuteNonQueryAsync();

            command.CommandText = createOrderMenuMappingsTable;
            await command.ExecuteNonQueryAsync();
        }

        public async Task<int> CreateRestaurantAsync(Restaurant restaurant)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                INSERT INTO Restaurants (Name, Cuisine, Rating, Image, Eta, Location, CreatedAt)
                VALUES (@Name, @Cuisine, @Rating, @Image, @Eta, @Location, @CreatedAt)
                RETURNING Id;";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Name", restaurant.Name);
            command.Parameters.AddWithValue("@Cuisine", restaurant.Cuisine);
            command.Parameters.AddWithValue("@Rating", restaurant.Rating);
            command.Parameters.AddWithValue("@Image", restaurant.Image ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Eta", restaurant.Eta);
            command.Parameters.AddWithValue("@Location", restaurant.Location);
            command.Parameters.AddWithValue("@CreatedAt", restaurant.CreatedAt);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<int> CreateMenuItemAsync(MenuItem menuItem)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                INSERT INTO MenuItems (Name, Description, Price, Image, Veg, CreatedAt)
                VALUES (@Name, @Description, @Price, @Image, @Veg, @CreatedAt)
                RETURNING Id;";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Name", menuItem.Name);
            command.Parameters.AddWithValue("@Description", menuItem.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Price", menuItem.Price);
            command.Parameters.AddWithValue("@Image", menuItem.Image ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Veg", menuItem.Veg);
            command.Parameters.AddWithValue("@CreatedAt", menuItem.CreatedAt);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task CreateRestaurantMenuMappingAsync(int restaurantId, int menuId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                INSERT INTO RestaurantMenuMappings (RestaurantId, MenuId)
                VALUES (@RestaurantId, @MenuId)
                ON CONFLICT (RestaurantId, MenuId) DO NOTHING;";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@RestaurantId", restaurantId);
            command.Parameters.AddWithValue("@MenuId", menuId);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<int> AddRestaurantWithMenuItemsAsync(Restaurant restaurant, List<MenuItem> menuItems)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Create restaurant
                var restaurantSql = @"
                    INSERT INTO Restaurants (Name, Cuisine, Rating, Image, Eta, Location, CreatedAt)
                    VALUES (@Name, @Cuisine, @Rating, @Image, @Eta, @Location, @CreatedAt)
                    RETURNING Id;";

                using var restaurantCommand = new NpgsqlCommand(restaurantSql, connection, transaction);
                restaurantCommand.Parameters.AddWithValue("@Name", restaurant.Name);
                restaurantCommand.Parameters.AddWithValue("@Cuisine", restaurant.Cuisine);
                restaurantCommand.Parameters.AddWithValue("@Rating", restaurant.Rating);
                restaurantCommand.Parameters.AddWithValue("@Image", restaurant.Image ?? (object)DBNull.Value);
                restaurantCommand.Parameters.AddWithValue("@Eta", restaurant.Eta);
                restaurantCommand.Parameters.AddWithValue("@Location", restaurant.Location);
                restaurantCommand.Parameters.AddWithValue("@CreatedAt", restaurant.CreatedAt);

                var restaurantId = Convert.ToInt32(await restaurantCommand.ExecuteScalarAsync());

                // Create menu items and mappings
                if (menuItems != null && menuItems.Any())
                {
                    var menuItemSql = @"
                        INSERT INTO MenuItems (Name, Description, Price, Image, Veg, CreatedAt)
                        VALUES (@Name, @Description, @Price, @Image, @Veg, @CreatedAt)
                        RETURNING Id;";

                    var mappingSql = @"
                        INSERT INTO RestaurantMenuMappings (RestaurantId, MenuId)
                        VALUES (@RestaurantId, @MenuId)
                        ON CONFLICT (RestaurantId, MenuId) DO NOTHING;";

                    foreach (var menuItem in menuItems)
                    {
                        // Insert menu item
                        using var menuItemCommand = new NpgsqlCommand(menuItemSql, connection, transaction);
                        menuItemCommand.Parameters.AddWithValue("@Name", menuItem.Name);
                        menuItemCommand.Parameters.AddWithValue("@Description", menuItem.Description ?? (object)DBNull.Value);
                        menuItemCommand.Parameters.AddWithValue("@Price", menuItem.Price);
                        menuItemCommand.Parameters.AddWithValue("@Image", menuItem.Image ?? (object)DBNull.Value);
                        menuItemCommand.Parameters.AddWithValue("@Veg", menuItem.Veg);
                        menuItemCommand.Parameters.AddWithValue("@CreatedAt", menuItem.CreatedAt);

                        var menuItemId = Convert.ToInt32(await menuItemCommand.ExecuteScalarAsync());

                        // Create mapping
                        using var mappingCommand = new NpgsqlCommand(mappingSql, connection, transaction);
                        mappingCommand.Parameters.AddWithValue("@RestaurantId", restaurantId);
                        mappingCommand.Parameters.AddWithValue("@MenuId", menuItemId);
                        await mappingCommand.ExecuteNonQueryAsync();
                    }
                }

                await transaction.CommitAsync();
                return restaurantId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }



        public async Task<List<Restaurant>> GetAllRestaurents()
        {
            var restaurants = new List<Restaurant>();
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                SELECT Id, Name, Cuisine, Rating, Image, Eta, Location, CreatedAt
                FROM Restaurants
                ORDER BY CreatedAt DESC;";

            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                restaurants.Add(new Restaurant
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    Cuisine = reader.GetString("Cuisine"),
                    Rating = reader.GetDecimal("Rating"),
                    Image = reader.IsDBNull("Image") ? string.Empty : reader.GetString("Image"),
                    Eta = reader.GetInt32("Eta"),
                    Location = reader.GetString("Location"),
                    CreatedAt = reader.GetDateTime("CreatedAt")
                });
            }

            return restaurants;
        }

        public async Task<List<MenuItem>> GetMenuItems(int restaurantID){
            List<MenuItem> menuItems = new ();
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var sql = @"select A.Id, A.Name, A.Description, A.Price, A.Image, A.Veg, A.CreatedAt from menuitems A 
                        inner join restaurantmenumappings B on A.ID = B.MenuID
                        where B.RestaurantID = @RestaurantID;"; 

            using var command = new NpgsqlCommand(sql,connection);
            command.Parameters.AddWithValue("@RestaurantID" , restaurantID);
            using var reader = await command.ExecuteReaderAsync();

            while(await reader.ReadAsync()){
                menuItems.Add(new MenuItem{
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    Description = reader.GetString("Description"),
                    Price = reader.GetInt32("Price"),
                    Image = reader.IsDBNull("Image") ? string.Empty : reader.GetString("Image"),
                    Veg = reader.GetBoolean("Veg"),
                    CreatedAt = reader.GetDateTime("CreatedAt")
                });
            }

            return menuItems;
        }


        public async Task<int> PlaceOrderRequest(Order order, List<int> mapping){
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try{
                var sql = @"INSERT INTO Orders(Status, RestaurantId, Amount,CreatedAt)
                Values(@Status, @RestaurantId, @Amount,  @CreatedAt)
                RETURNING OrderId;";

                using var orderCommand = new NpgsqlCommand(sql, connection, transaction);
                orderCommand.Parameters.AddWithValue("@Status", "Pending");
                orderCommand.Parameters.AddWithValue("@RestaurantId", order.RestaurantId);
                orderCommand.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                orderCommand.Parameters.AddWithValue("@Amount", order.Amount);
                var orderId = (int)await orderCommand.ExecuteScalarAsync();

                var mappingSql = @"
                INSERT INTO ordermenumappings (OrderId, MenuId)
                VALUES (@OrderId, @MenuId);";

                foreach(var item in mapping){
                    using var command = new NpgsqlCommand(mappingSql, connection, transaction);
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    command.Parameters.AddWithValue("@MenuId", item);
                    await command.ExecuteNonQueryAsync();
                }
                await transaction.CommitAsync();
                return orderId;
                
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }
    }
}

