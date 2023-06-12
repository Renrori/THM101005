using DeliveryBro.Extensions;
using DeliveryBro.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Validations;
using System.Collections.Concurrent;

namespace DeliveryBro.Hubs
{
    [Authorize(AuthenticationSchemes = "DeliverAuthenticationScheme")]
    [Authorize(AuthenticationSchemes = "CustomerAuthenticationScheme")]
    [Authorize(AuthenticationSchemes = "StoreAuthenticationScheme")]
    public class OrderHub : Hub
    {
        public static Dictionary<Guid, string> AllUser = new Dictionary<Guid, string>();
        public async Task Infomation(Guid customerId, int orderId, string status)
        {
            var conn = AllUser[customerId];
            if (conn != null)
                await Clients.Client(conn).SendAsync("orderStatus", orderId, status);
        }
        public override async Task OnConnectedAsync()
        {
            var u = Context.User.GetId();
            var isExist = AllUser.FirstOrDefault(x => x.Key == u);
            if (isExist.Key != null) AllUser[u] = Context.ConnectionId;
            else AllUser.Add(u, Context.ConnectionId);
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
