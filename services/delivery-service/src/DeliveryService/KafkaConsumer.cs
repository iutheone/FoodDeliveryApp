using Confluent.Kafka;
using System.Text.Json;


namespace DeliveryService.Kafka{
  public class KafkaConsumer : BackgroundService{
    private readonly string _topic= "order.placed";
    private readonly string _groupId = "delivery-service-group";
    private readonly string _bootstrapServers;
    private readonly ILogger<KafkaConsumer> _logger;

    public KafkaConsumer(IConfiguration config, ILogger<KafkaConsumer> logger)
    {
      _bootstrapServers = config.GetValue<string>("Kafka:BootstrapServers") ?? "localhost:29092";
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken){
      _logger.LogInformation("Starting Kafka consumer");
      var config = new ConsumerConfig{
        GroupId = _groupId,
        BootstrapServers = _bootstrapServers,
        AutoOffsetReset = AutoOffsetReset.Earliest
      };
      using var consumer = new ConsumerBuilder<Null, string>(config).Build();
        consumer.Subscribe(_topic);
       _logger.LogInformation("🚚 DeliveryService is listening for order events...");

       try{
        while(!stoppingToken.IsCancellationRequested){
          var cr = consumer.Consume(stoppingToken);
          var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(cr.Message.Value);
          if (orderEvent != null)
          {
              _logger.LogInformation($"📦 Order Received: {orderEvent.OrderId} for {orderEvent.CustomerName}");
              await HandleDeliveryAssignment(orderEvent);
          }else{
            return;
          }
        }
       }catch(Exception ex){
        _logger.LogError(ex, "Error in Kafka consumer");
         consumer.Close();
       }
    }

    private async Task HandleDeliveryAssignment(OrderCreatedEvent order)
    {
        _logger.LogInformation($"🛵 Assigning delivery partner for Order {order.OrderId}...");

        await Task.Delay(20000); // simulate delay

        _logger.LogInformation($"✅ Delivery Partner Assigned for Order {order.OrderId}");
    }
    
  }


  public class OrderCreatedEvent
  {
      public Guid OrderId { get; set; }
      public string CustomerName { get; set; } = string.Empty;
      public string Restaurant { get; set; } = string.Empty;
      public decimal Amount { get; set; }
      public string Status { get; set; } = string.Empty;
      public DateTime CreatedAt { get; set; }
  }
}