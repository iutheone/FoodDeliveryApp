
using Microsoft.AspNetCore.Mvc;
using DeliveryService.Repository;

namespace DeliveryService.Controller{
  [ApiController]
  [Route("/api/Restaurant")]
  public class RestaurantController : ControllerBase{
    private readonly Repository _repository;

    public RestaurantController(Repository repository)
    {
      _repository = repository;
    }

    [HttpGet("GetAllOrdersWithDetails")]
    public async Task<IActionResult> GetAllOrdersWithDetails()
    {
      try
      {
        var ordersWithDetails = await _repository.GetAllOrdersWithDetailsAsync();
        return Ok(new { Orders = ordersWithDetails, Message = "Success" });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Error getting orders with details", error = ex.Message });
      }
    }
  }
}
