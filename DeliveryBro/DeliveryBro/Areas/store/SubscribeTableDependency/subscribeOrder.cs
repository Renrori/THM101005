using DeliveryBro.Areas.store.Hubs;
using DeliveryBro.MetaData;
using Microsoft.AspNetCore.SignalR;
using TableDependency.SqlClient;

namespace DeliveryBro.Areas.store.SubscribeTableDependency
{
	public class subscribeOrder
	{
		SqlTableDependency<CustomerOrder_Table> td;
		private readonly IHubContext<OrderHub> _hubContext;
		public subscribeOrder(IHubContext<OrderHub> hubContext)
		{
			
			_hubContext = hubContext;
		}

		public void Subscribe()
		{
			string conn = "Server=sql8005.site4now.net;Database=db_a989fc_thm101005;User Id=db_a989fc_thm101005_admin;Password=THM101team5;MultipleActiveResultSets=true";
			td = new SqlTableDependency<CustomerOrder_Table>(conn);
			td.OnChanged += Td_OnChanged;
			td.Start();
		}

		private void Td_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<CustomerOrder_Table> e)
		{
			_hubContext.Clients.All.SendAsync("NewOrder");
		}
	}
}
