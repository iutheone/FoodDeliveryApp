using DeliveryService.Kafka;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHostedService<KafkaConsumer>();

var app = builder.Build();

app.MapControllers();
app.UseHttpsRedirection();

app.Run();
