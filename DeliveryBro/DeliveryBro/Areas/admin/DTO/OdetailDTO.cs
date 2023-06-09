using DeliveryBro.Areas.store.DTO;

namespace DeliveryBro.Areas.admin.DTO
{
	public class OdetailDTO
	{
		public int OrderId { get; set; }
		public string OrderDate { get; set; }
		public string CompletedTime { get; set; }
		public string CustomerName { get; set; }
		public string Note { get; set; }
		public int Total { get; set; }
		public ICollection<OrderDetailsDTO> OrderDetails { get; set; }
	}
}
