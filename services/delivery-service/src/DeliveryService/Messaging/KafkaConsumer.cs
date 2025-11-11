using Confluent.Kafka;
using System.Text.Json;
using Confluent.Kafka.Admin;


namespace DeliveryService.Kafka{
  public class KafkaConsumer : BackgroundService{
    private readonly string _topic= "order.placed";
    private readonly string _groupId = "delivery-service-group";
    private readonly string _bootstrapServers;
    private readonly ILogger<KafkaConsumer> _logger;

    public KafkaConsumer(IConfiguration config, ILogger<KafkaConsumer> logger)
    {
      _bootstrapServers = config.GetValue<string>("Kafka:BootstrapServers") ?? "kafka:9092";
      _logger = logger;
    }

    private async Task EnsureTopicExistsAsync()
    {
        try
        {
            var adminConfig = new AdminClientConfig { BootstrapServers = _bootstrapServers };
            using var adminClient = new AdminClientBuilder(adminConfig).Build();
            
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            var topicExists = metadata.Topics.Any(t => t.Topic == _topic);
            
            if (!topicExists)
            {
                _logger.LogInformation($"Topic '{_topic}' does not exist. Creating it...");
                try
                {
                    await adminClient.CreateTopicsAsync(new TopicSpecification[]
                    {
                        new TopicSpecification
                        {
                            Name = _topic,
                            NumPartitions = 1,
                            ReplicationFactor = 1
                        }
                    });
                    _logger.LogInformation($"✅ Topic '{_topic}' created successfully");
                }
                catch (CreateTopicsException ex)
                {
                    if (ex.Results[0].Error.Code != ErrorCode.TopicAlreadyExists)
                    {
                        _logger.LogError(ex, $"Error creating topic '{_topic}'");
                        throw;
                    }
                    _logger.LogInformation($"Topic '{_topic}' already exists (created by another service)");
                }
            }
            else
            {
                _logger.LogInformation($"Topic '{_topic}' already exists");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Could not verify/create topic '{_topic}'. Will attempt to proceed anyway.");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken){
      _logger.LogInformation("Starting Kafka consumer");
      
      // Ensure topic exists before subscribing
      await EnsureTopicExistsAsync();
      
      var config = new ConsumerConfig{
        GroupId = _groupId,
        BootstrapServers = _bootstrapServers,
        AutoOffsetReset = AutoOffsetReset.Earliest
      };
      
      using var consumer = new ConsumerBuilder<Null, string>(config).Build();
      
      try
      {
        consumer.Subscribe(_topic);
        _logger.LogInformation("🚚 DeliveryService is listening for order events...");

        while(!stoppingToken.IsCancellationRequested){
          try
          {
            var cr = consumer.Consume(stoppingToken);
            var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(cr.Message.Value);
            if (orderEvent != null)
            {
                _logger.LogInformation($"📦 Order Received: {orderEvent.OrderId} for {orderEvent.CustomerName}");
                await HandleDeliveryAssignment(orderEvent);
            }
          }
          catch (ConsumeException ex)
          {
            _logger.LogError(ex, $"Error consuming message: {ex.Error.Reason}");
          }
        }
      }
      catch(Exception ex)
      {
        _logger.LogError(ex, "Error in Kafka consumer");
      }
      finally
      {
        consumer.Close();
      }
    }

    private async Task HandleDeliveryAssignment(OrderCreatedEvent order)
    {
        _logger.LogInformation($"🛵 Assigning delivery partner for Order {order.OrderId}...");

        await Task.Delay(20000); // simulate delay

        _logger.LogInformation($"✅ Delivery Partner Assigned for Order {order.OrderId}");

         await Task.Delay(20000); // simulate delay


    }

    private async SendDeliveryStatus(OrderCreatedEvent order){
       
        var deliveryStatusProducer = new DeliveryStatusProducer(_bootstrapServers);
        deliveryStatusProducer.ProduceAsync();
        await Task.Delay(20000); // simulate delay


       
    }
    
  }

  public record DeliveryStatus(string OrderID, string status);



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