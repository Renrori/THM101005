namespace DeliveryBro.Areas.admin.Models
{
	public class OrderViewModel
	{
		public int OrderID { get; set; }
		public string CustomerAdress { get; set; }

		public string ShippingFee { get; set; }

		public string Payment { get; set;}

		public string OrderDate { get; set; }
		public Guid? CouponID { get; set; }

		public string Discount { get; set; }

		public string AmountAfterDiscount { get; set; }

		public string note { get;set; }

		public Guid? DriverID { get; set; }

		public Guid? CustomerID { get; set; }

		public Guid? RestaurantID { get; set; }

		public int Total { get; set; }

		






	}
}
