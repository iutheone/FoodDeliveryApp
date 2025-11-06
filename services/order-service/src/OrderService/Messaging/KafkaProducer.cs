using Confluent.Kafka;
using System.Text.Json;

namespace OrderService.Messaging{
  public class KafkaProducer : IDisposable{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic;
    public KafkaProducer(string bootstrapServers, string topic)
    {
      _topic = topic;
      var conf = new ProducerConfig { BootstrapServers = bootstrapServers };
      _producer = new ProducerBuilder<Null, string>(conf).Build();
    }

      public async Task ProduceAsync<T>(T message)
      {
          var json = JsonSerializer.Serialize(message);
          var result = await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = json });
          Console.WriteLine($"✅ Sent message to Kafka: {json}");
      }

    public void Dispose() => _producer?.Flush(TimeSpan.FromSeconds(5));
  }
}