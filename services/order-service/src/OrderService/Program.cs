

using OrderService.Data;
using OrderService.Messaging;

var builder = WebApplication.CreateBuilder(args);

// config
var kafkaBootStrapServer = builder.Configuration.GetValue<string>("Kafka:BootstrapServers")?? "kafka:9092";
var kafkaTopic = builder.Configuration.GetValue<string>("Kafka:Topic") ?? "order.placed";
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=postgres;Database=ordersdb;Username=postgres;Password=postgres";

//Add ADO.NET Repository
builder.Services.AddScoped<IOrderRepository>(sp => new OrderRepository(connStr));

//Add Kafka Producer
builder.Services.AddSingleton<KafkaProducer>(new KafkaProducer(kafkaBootStrapServer, kafkaTopic));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
    await repository.InitializeDatabaseAsync();
}

await app.RunAsync();
