namespace DeliveryBro.Areas.admin.DTO
{
	public class OrderBasicDTO
	{
		public int? OrderID { get; set; }
		public string? CustomerAdress { get; set; }

		public string? ShippingFee { get; set; }

		public string? Payment { get; set; }

		public string? OrderDate { get; set; }
		public Guid? CouponID { get; set; }

		public string? Discount { get; set; }

		public string? AmountAfterDiscount { get; set; }

		public Guid? DriverID { get; set; }

		public Guid? CustomerID { get; set; }

		public Guid? RestaurantID { get; set; }

		public string? OrderStatus { get; set; }

		public int? Total { get; set; }

		public string? CompletedTime { get; set; }
		public string? CustomerName { get; set; }
		public string? Note { get; set; }

		public int? UnitPrice { get; set; }

		public int? Quantity { get; set; }

		public string? RestaurantName { get; set; }

		public string? DishName { get; set; }

		public int? Subtotal { get; set; }

		public string? DriverName { get; set; }
		public string? Address { get; set; }
	}
}
