namespace DeliveryBro.Areas.store.DTO
{
	public class HisOrderDTO
	{

			public int OrderId { get; set; }
			public DateTime OrderDate { get; set; }
			//public string DishName { get; set; }
			public string CustomerName { get; set; }
			public string Note { get; set; }
			public int Total { get; set; }
			public ICollection<OrderDetailsDTO> OrderDetails { get; set; }
    }
}
