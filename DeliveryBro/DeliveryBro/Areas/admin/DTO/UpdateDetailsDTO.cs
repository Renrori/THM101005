namespace DeliveryBro.Areas.admin.DTO
{
	public class UpdateDetailsDTO
	{
		public int? OrderID { get; set; }
		public string? OrderName { get; set; }

		public DateTime? OrderDate { get; set; }

		public string? OrderStatus { get; set; }
		public string? CustomerAddress { get; set; }

		public string? Note { get; set; }

	}
}
