
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

    private readonly OrdersDbContext _context;
    private readonly KafkaProducer _kafkaProducer;
    public OrdersController(OrdersDbContext context, KafkaProducer kafkaProducer)
    {
      _context = context;
      _kafkaProducer = kafkaProducer;
    }

    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest req){
      var order = new Order{
        OrderId = Guid.NewGuid(),
        UserId = req.UserId,
        Amount = req.Amount,
        Status = "Placed",
        CreatedAt = DateTime.UtcNow
      };
      _context.Orders.Add(order);
      await _context.SaveChangesAsync();
      await _kafkaProducer.ProduceAsync(new OrderPlacedEvent(order.OrderId, order.UserId, order.Amount, order.CreatedAt));
      return Ok(new { orderId = order.OrderId });
    }

    public record PlaceOrderRequest(Guid UserId, decimal Amount);
  }
}