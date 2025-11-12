using DeliveryService.Kafka;
using DeliveryService.Repository;

var builder = WebApplication.CreateBuilder(args);

// Get connection string from configuration
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=postgres;Database=ApplicationDB;Username=postgres;Password=postgres";

// Register repository
builder.Services.AddScoped<Repository>(sp => new Repository(connStr));

builder.Services.AddControllers();
builder.Services.AddHostedService<KafkaConsumer>();

var app = builder.Build();

app.MapControllers();
app.UseHttpsRedirection();

app.Run();
