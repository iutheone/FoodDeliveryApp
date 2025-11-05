
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;


namespace OrderTrackingService.Hubs{
  public class OrderTrackingHub: Hub{
    public async Task SubscribeToOrder(string orderId){
      await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
    }

    public async Task UnsubscribeFromOrder(string orderId){
      await Groups.RemoveFromGroupAsync(Context.ConnectionId, orderId);
    }
  }
}