
using OrderTrackingService.Hubs;
using OrderTrackingService.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddHostedService<OrderTrackingService>();

var app = builder.Build();

app.MapHub<OrderTrackingHub>("/order-tracking");
app.MapControllers();

app.Run();
