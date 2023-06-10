using DeliveryBro.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DeliveryBro.Services
{
	public class OrderNotificationTask
	{
		private readonly IHubContext<OrderHub> _hubContext;
		public OrderNotificationTask(IHubContext<OrderHub> hubContext)
		{
			_hubContext = hubContext;
		}
		public void Notify()
		{
			_hubContext.Clients.All.SendAsync("NewOrder");
		}
	}
}
