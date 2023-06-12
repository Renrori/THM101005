namespace DeliveryBro.Areas.admin.Models.ViewModel
{
    public class OrderManagementDTO
    {
        public int Count { get; set; }
		public List<OrderDTO> Items { get; set; }

    }

    public class OrderDTO {
		public int OrderId { get; set; }
		public DateTime OrderDate { get; set; }
		public string CustomerAddress { get; set; }
		public int AmountAfterDiscount { get; set; }
		public string OrderStatus { get; set; }
		public string Payment { get; set; }
	}
	public class OrderEditDTO
	{
		public int OrderId { get; set; }
		public DateTime OrderDate { get; set; }
		public string CustomerAddress { get; set; }
		public int AmountAfterDiscount { get; set; }
		//public string OrderStatus { get; set; }
	}
}
