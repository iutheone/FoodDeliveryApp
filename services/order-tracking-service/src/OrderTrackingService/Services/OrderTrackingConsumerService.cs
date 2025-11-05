
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderTrackingService.Hubs;
using System.Text.Json;


namespace OrderTrackingService.Services{
  public class OrderTrackingService: BackgroundService{
    private readonly IConsumer<null, string> _consumer;
    private readonly ILogger<OrderTrackingService> _logger;
    private readonly IHubContext<OrderTrackingHub> _hubContext;
    private readonly string _topic;
  }

  public OrderTrackingService(IHubContext<OrderTrackingHub> hubContext, ILogger<KafkaConsumerService> logger, IConfiguration config){
    _hubContext = hubContext;
    _logger = logger;
    _topic = config.GetValue<string>("Kafka:Topic")
    var config = new ConsumerConfig{
      BootstrapServers =  config.GetValue<string>("Kafka:BootstrapServers") ?? "kafka:9092";
      GroupId = "tracking-service-group",
      AutoOffsetReset = AutoOffsetReset.Earliest
    }
    _consumer = new ConsumerBuilder<Null, string>(config).Build();
  }


  protected override async Task ExecuteAsync(CancellationToken cancellationToken){
    _consumer.Subscribe(_topic);
    _logger.LogInformation("Listening to Kafka topic: delivery-updated");
    while(!cancellationToken.IsCancellationRequested){
      try
      {
        var result = _consumer.Consume(cancellationToken);
        if (result == null) continue;
        var message = result.Message.Value;
        _logger.LogInformation($"Received delivery update: {message}");

        var update = JsonSerializer.Deserialize<Dictionary<string, string>>(message);
        if (update != null && update.ContainsKey("OrderId"))
        {
            await _hubContext.Clients.Group(update["OrderId"])
                .SendAsync("ReceiveOrderUpdate", update);
        }
      }
      catch (System.Exception ex)
      {
        _logger.LogError($"Kafka error: {ex.Message}");
        throw;
      }
    }
  }

  
}