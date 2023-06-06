using Microsoft.AspNetCore.Http.Connections;

namespace DeliveryBro.ViewModels
{
	public class UserInfo
	{
		public string ConnectionId { get; set; }
		public Guid UserId { get; set; }
		public string Name { get; set; }
		public string Role { get; set; }
	}
	public class ChatMessage
	{
		public UserInfo Sender { get; set;}
		public UserInfo Receiver { get; set; }
		public string Message { get; set; }
		public long TimeStamp { get; set; }
		public bool read { get; set; }
	}
}
