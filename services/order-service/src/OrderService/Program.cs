

using OrderService.Data;
using OrderService.Messaging;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// config
var kafkaBootStrapServer = builder.Configuration.GetValue<string>("Kafka:BootstrapServers")?? "localhost:29092";
var kafkaTopic = builder.Configuration.GetValue<string>("Kafka:Topic") ?? "order.placed";
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Database=ordersdb;Username=postgres;Password=postgres";


//Add EF Core
builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseNpgsql(connStr)
);

//Add Kafka Producer
builder.Services.AddSingleton<KafkaProducer>(new KafkaProducer(kafkaBootStrapServer, kafkaTopic));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
// EF migrations / ensure db created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
