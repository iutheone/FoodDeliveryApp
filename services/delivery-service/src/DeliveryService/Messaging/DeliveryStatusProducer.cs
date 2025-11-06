using Confluent.Kafka;
using System.Text.Json;
using Confluent.Kafka.Admin;

namespace DeliveryService.Kafka{
  public class DeliveryStatusProducer: IDisposable{
    private readonly string _topic= "delivery-status";
    private readonly string _bootstrapServers;
    private readonly ILogger<DeliveryStatusProducer> _logger;

    public DeliveryStatusProducer(string bootstrapServers)
    {
      _topic = "delivery-status";
      var conf = new ProducerConfig { BootstrapServers = bootstrapServers };
      _producer = new ProducerBuilder<Null, string>(conf).Build();
    }

    public async Task ProduceAsync<T>(T message)
    {
        var json = JsonSerializer.Serialize(message);
        var result = await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = json });
        Console.WriteLine($"✅ Sent message to Kafka: {json}");
    }

  }
}