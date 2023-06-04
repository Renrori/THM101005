using DeliveryBro.Extensions;
using DeliveryBro.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Security.Claims;

namespace DeliveryBro.Hubs
{
    [Authorize(AuthenticationSchemes = "CustomerAuthenticationScheme")]
    [Authorize(AuthenticationSchemes = "StoreAuthenticationScheme")]
    [Authorize(AuthenticationSchemes = "AdministratorAuthenticationScheme")]
    public class ChatHub : Hub
    {
        public static List<Dictionary<Guid, string>> conn = new List<Dictionary<Guid, string>>();

        public override async Task OnConnectedAsync()
        {
            Dictionary<Guid, string> storedict = new Dictionary<Guid, string>();
            Dictionary<Guid, string> cusdict = new Dictionary<Guid, string>();
            //int i = provider.GetUserId(Context);
            conn.Add(storedict);
            conn.Add(cusdict);
            
            var id = Context.User.GetId(Context.User.GetRole());
            string role = Context.User.GetRole();
            string name = Context.User.GetName();
            string adminGroup = "adminGroup";
            if (role == "Administrator")
                await Groups.AddToGroupAsync(Context.ConnectionId, adminGroup);
            else if (role == "Store")
            {
                if (conn[0].ContainsKey(id)) conn[0][id] = Context.ConnectionId;
                else conn[0].Add(id, Context.ConnectionId);
            }
            else
            {
                if (conn[1].ContainsKey(id)) conn[1][id] = Context.ConnectionId;
                else conn[1].Add(id, Context.ConnectionId);
            }
            //string jsonString = JsonConvert.SerializeObject(conn);
            //await Clients.All.SendAsync("UpdList", jsonString);

            await Clients.Client(Context.ConnectionId).SendAsync("UpdSelfID", Context.ConnectionId);

            await Clients.Group("adminGroup").SendAsync("UpdContent", $"新連線: {name}"  + Context.ConnectionId);

            await base.OnConnectedAsync();
            
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string name = Context.User.GetName();
            await Clients.All.SendAsync("UpdContent", $"已離線: {name}" + Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string message)
        {
            var id = Context.User.GetId(Context.User.GetRole());
            string role = Context.User.GetRole();
            string connectionId;
            string name = Context.User.GetName();
            if (role == "Store") connectionId = conn[0][id];
            else connectionId = Context.ConnectionId;
            await Clients.Client(connectionId).SendAsync("updmsg", message, name);
            await Clients.Group("adminGroup").SendAsync("SendtoAdmin",message,name);
        }
        public async Task SendMessagetoCaller(string message,string connectionid)
        {
            
            await Clients.Client(connectionid).SendAsync("SendtoCaller",message);
        }
    }
}
