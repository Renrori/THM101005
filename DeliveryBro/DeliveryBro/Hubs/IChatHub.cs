using DeliveryBro.ViewModels;
using System.Collections.Concurrent;

namespace DeliveryBro.Hubs
{
	public interface IChatHub
	{
		Task SendSelfId(string connectionID, string guid, string name,string role);
		Task ToAdminMessage(string connectionID,string guid,string name,string role, string message,bool read);
		Task ToStoreMessage(string connectionID, string guid, string name, string role, string message,bool read);
		Task UserIsOnline(string connectionID, string guid, string name, string role);
		Task UserIsOffline(string connectionID, string guid, string name, string role);
		Task AdminIsOnline(string adminsJson);
		Task AdminOn(string connectionID, string guid, string name, string role);
		Task AdminIsOff(string connectionID, string guid, string name, string role);
		Task GetAllOffline(string offineUsersJson);
		Task GetHistoryMessage(string messageJson);
		Task StoreGetHistoryMessage(string messageJson);
		Task GetAllOnline(string allUsersJson);
		Task NotifyNewMessage(string guid);
		Task UnReadMessage(List<string> guid);
	}
}
