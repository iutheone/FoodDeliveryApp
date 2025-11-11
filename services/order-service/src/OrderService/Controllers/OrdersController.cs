
using Microsoft.AspNetCore.Mvc;
using OrderService.Data;
using OrderService.Models;
using OrderService.Messaging;
using Shared.Events;

namespace OrderService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class OrdersController : ControllerBase
  {

    private readonly IOrderRepository _repository;
    private readonly KafkaProducer _kafkaProducer;
    public OrdersController(IOrderRepository repository, KafkaProducer kafkaProducer)
    {
      _repository = repository;
      _kafkaProducer = kafkaProducer;
    }

    [HttpGet]
    public IActionResult Get(){
      return Ok("working fine..");
    }

    [HttpPost("AddRestaurant")]
    public async Task<IActionResult> AddRestaurant([FromBody] AddRestaurantRequest request)
    {
      try
      {
        // Create restaurant object
        var restaurant = new Restaurant
        {
          Name = request.Name,
          Cuisine = request.Cuisine,
          Rating = request.Rating,
          Image = request.Image,
          Eta = request.Eta,
          Location = request.Location,
          CreatedAt = DateTime.UtcNow
        };

        // Prepare menu items
        var menuItems = new List<MenuItem>();
        if (request.MenuItems != null && request.MenuItems.Any())
        {
          foreach (var menuItemDto in request.MenuItems)
          {
            var menuItem = new MenuItem
            {
              Name = menuItemDto.Name,
              Description = menuItemDto.Description,
              Price = menuItemDto.Price,
              Image = menuItemDto.Image,
              Veg = menuItemDto.Veg,
              CreatedAt = DateTime.UtcNow
            };
            menuItems.Add(menuItem);
          }
        }

        // Add restaurant with menu items (transaction handled in repository)
        var restaurantId = await _repository.AddRestaurantWithMenuItemsAsync(restaurant, menuItems);

        return Ok(new { message = "Restaurant and menu items added successfully", restaurantId = restaurantId });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Error adding restaurant", error = ex.Message });
      }
    }

    // [HttpPost("PlaceOrder")]
    // public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest req){
    //   var order = new Order{
    //     OrderId = Guid.NewGuid(),
    //     Status = "Placed",
    //     CreatedAt = DateTime.UtcNow
    //   };
    //   _context.Orders.Add(order);
    //   //await _context.SaveChangesAsync();
    //   await _kafkaProducer.ProduceAsync(new OrderPlacedEvent(order.OrderId, order.Amount, order.CreatedAt));
    //   return Ok(new { orderId = order.OrderId });
    // }

    public record AddRestaurantRequest(
      string Name,
      string Cuisine,
      decimal Rating,
      string Image,
      int Eta,
      string Location,
      List<MenuItem>? MenuItems
    );
  }
}