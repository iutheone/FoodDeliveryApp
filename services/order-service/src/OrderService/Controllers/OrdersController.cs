
using Microsoft.AspNetCore.Mvc;
using OrderService.Data;
using SharedEvents.Models;
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
        var restaurantId = await _repository.AddRestaurantWithMenuItemsAsync(restaurant, menuItems);

        return Ok(new { message = "Restaurant and menu items added successfully", restaurantId = restaurantId });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Error adding restaurant", error = ex.Message });
      }
    }

    [HttpGet("GetAllRestuarants")]
    public async Task<IActionResult> GetAllRestuarants(){
      try
      {
        var restaurants =  _repository.GetAllRestaurents();
        return Ok(new {Restaurants = restaurants, Message = "Success"});
      }
      catch (System.Exception ex)
      { 
        return StatusCode(500, new { message = "Error in Getting All Restaurant list", error = ex.Message });
      }
    }

    [HttpGet("GetRestaurantMenuList")]
    public async Task<IActionResult> GetRestaurantMenuList([FromQuery] int restId){
      try
      {
        var menuList =  _repository.GetMenuItems(restId);
        return Ok(new {MenuList = menuList, Message = "Success"});
      }
      catch (System.Exception ex)
      { 
        return StatusCode(500, new { message = "Error in Getting Restaurant Menu List ", error = ex.Message });
      }
    }

    [HttpPost("PlaceOrder")]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest req){
      try
      { 
        var orderId = _repository.PlaceOrderRequest(new Order{RestaurantId = req.RestaurantId, Amount = req.Amount}, req.MenuItems);
        await _kafkaProducer.ProduceAsync(new OrderPlacedEvent(orderId, req.Amount, DateTime.UtcNow));
        return Ok(new { orderId = orderId, message= "Order placed successfully" });
      }
      catch(Exception ex){
        return StatusCode(500, new { message = "Error in Getting Restaurant Menu List ", error = ex.Message });
      }
      //
    }

    public record AddRestaurantRequest(
      string Name,
      string Cuisine,
      decimal Rating,
      string Image,
      int Eta,
      string Location,
      List<MenuItem>? MenuItems
    );

    public record PlaceOrderRequest(
      int RestaurantId,
      int Amount,
      List<int> MenuItems
    );
  }
}