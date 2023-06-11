using DeliveryBro.Areas.store.DTO;

namespace DeliveryBro.Areas.admin.DTO
{
	public class OrderdetailsDTO
	{
		public int OrderId { get; set; }

		public string OrderDate { get; set; }
		public string CompletedTime { get; set; }
		public string CustomerName { get; set; }
		public string Note { get; set; }

		public int UnitPrice { get; set; }

		public int Quantity { get; set; }
		public decimal Total { get; set; }

		public string RestaurantName { get; set; }

		public string DishName { get; set; }
		public int Discount { get; set; }

		public int Subtotal { get; set;}

		public int DriverID { get; set; }
		public string DriverName { get; set; }
		public string Address { get; set; }	
		public string Payment { get; set; }
	}
}
