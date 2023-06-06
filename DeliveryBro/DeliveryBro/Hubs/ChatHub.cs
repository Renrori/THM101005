using DeliveryBro.Extensions;
using DeliveryBro.Services;
using DeliveryBro.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DeliveryBro.Hubs
{
    [Authorize(AuthenticationSchemes = "CustomerAuthenticationScheme")]
    [Authorize(AuthenticationSchemes = "StoreAuthenticationScheme")]
    [Authorize(AuthenticationSchemes = "AdministratorAuthenticationScheme")]
    public class ChatHub : Hub<IChatHub>
    {
        public static ConcurrentDictionary<Guid, UserInfo> AllUser = new ConcurrentDictionary<Guid, UserInfo>();
		public static ConcurrentDictionary<Guid, UserInfo> OfflineUser = new ConcurrentDictionary<Guid, UserInfo>();
		public static ConcurrentBag<ChatMessage> chatMessages = new ConcurrentBag<ChatMessage>();
        public async Task SendMessagetoAdmin(string message)
		{
			var u = Context.User.GetInfo();
			u.ConnectionId = Context.ConnectionId;
			await Clients.Group("adminGroup").ToAdminMessage(u.ConnectionId, u.UserId.ToString(), u.Name, u.Role, message,false);
			var admins = AllUser.Where(x => x.Value.Role == "Administrator").ToList();			
			foreach (var admin in admins)
			{
				chatMessages.Add(new ChatMessage
				{
					Message = message,
					TimeStamp = DateTime.UtcNow.Ticks,
					Sender = u,
					Receiver = admin.Value,
					read=false
				});
			}
			await Clients.Group("adminGroup").NotifyNewMessage(u.UserId.ToString());
		}
		public async Task SendMessagetoCaller(string message, string connectionid,string userid)
        {
            var u=Context.User.GetInfo();
			var r = AllUser.FirstOrDefault(x => x.Key == new Guid(userid));
			if(r.Value==null) r=OfflineUser.FirstOrDefault(x=>x.Key == new Guid(userid));
			await Clients.Client(userid).ToStoreMessage(u.ConnectionId, u.UserId.ToString(), u.Name, u.Role, message,false);			
			chatMessages.Add(new ChatMessage
			{
				Message = message,
				TimeStamp = DateTime.UtcNow.Ticks,
				Sender = u,
				Receiver = r.Value,
				read=false
			});

		}
		public async Task UnRead()
		{
			var unReadMsgFromWho = chatMessages.Where(x => x.read == false).GroupBy(x => x.Sender.UserId).Select(q => q.Key.ToString()).ToList();
			await Clients.Group("adminGroup").UnReadMessage(unReadMsgFromWho);
		}
		public async Task ReadStatus(string guid)
		{
			var Messages=chatMessages.Where(x=>x.Sender.UserId.ToString() == guid);
			foreach(var message in Messages) 
				message.read = true;
		}
		public async Task GetHistoryMessage(string guid)
		{
			var u = Context.User.GetInfo();
			var ourMessage = chatMessages.Where(x=> (x.Sender.UserId == u.UserId && x.Receiver.UserId == new Guid(guid)) ||
			(x.Receiver.UserId == u.UserId && x.Sender.UserId == new Guid(guid))).OrderBy(x=>x.TimeStamp);
			await Clients.Caller.GetHistoryMessage(JsonConvert.SerializeObject(ourMessage));
		}

		#region 連線離線
		public override async Task OnConnectedAsync()
		{
			var userInfo = Context.User.GetInfo();
			userInfo.ConnectionId = Context.ConnectionId;

			if (userInfo.Role == "Administrator")
				await Groups.AddToGroupAsync(Context.ConnectionId, "adminGroup");
			var Admins=AllUser.Where(x=>x.Value.Role== "Administrator");
			
			var chathistory = chatMessages.Where(x => (x.Sender.UserId == userInfo.UserId&&x.Receiver.UserId==Admins.FirstOrDefault(a=>a.Key==x.Receiver.UserId).Key) 
							|| (x.Receiver.UserId == userInfo.UserId&&x.Sender.UserId==Admins.FirstOrDefault(a=>a.Key==x.Sender.UserId).Key))
							.OrderBy(x=>x.TimeStamp);
			if (chathistory != null)
				OfflineUser.TryRemove(userInfo.UserId, out var delUser);
			AllUser.AutoAddOrUpdate(userInfo.UserId, userInfo);
			await Clients.Client(Context.ConnectionId).SendSelfId(Context.ConnectionId,userInfo.UserId.ToString(), userInfo.Name, userInfo.Role);
			await Clients.Client(Context.ConnectionId).StoreGetHistoryMessage(JsonConvert.SerializeObject(chathistory));
			await Clients.Group("adminGroup").GetAllOnline(JsonConvert.SerializeObject(AllUser.Select(x=>x.Value)));
			await Clients.Group("adminGroup").UserIsOnline(Context.ConnectionId, userInfo.UserId.ToString(), userInfo.Name, userInfo.Role);
			await Clients.Group("adminGroup").GetAllOffline(JsonConvert.SerializeObject(OfflineUser.Select(x => x.Value)));
			if(userInfo.Role== "Administrator")
			await Clients.All.AdminOn(Context.ConnectionId, userInfo.UserId.ToString(), userInfo.Name, userInfo.Role);
			await Clients.Caller.AdminIsOnline(JsonConvert.SerializeObject(Admins.Select(x => x.Value)));
			await base.OnConnectedAsync();
		}
		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			var userInfo = Context.User.GetInfo();
			var chathistory = chatMessages.FirstOrDefault(x => x.Sender.UserId == userInfo.UserId || x.Receiver.UserId == userInfo.UserId);
			if (chathistory != null)
				OfflineUser.AutoAddOrUpdate(userInfo.UserId, userInfo);
			AllUser.TryRemove(userInfo.UserId, out var delUser);
			await Clients.Group("adminGroup").UserIsOffline(Context.ConnectionId, userInfo.UserId.ToString(), userInfo.Name, userInfo.Role);
			await Clients.Group("adminGroup").GetAllOffline(JsonConvert.SerializeObject(OfflineUser.Select(x => x.Value)));
			if(userInfo.Role== "Administrator")
				await Clients.All.AdminIsOff(Context.ConnectionId, userInfo.UserId.ToString(), userInfo.Name, userInfo.Role);
			await base.OnDisconnectedAsync(exception);
		}
		#endregion
	}
}
