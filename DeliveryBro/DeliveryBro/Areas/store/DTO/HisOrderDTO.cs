﻿using System.ComponentModel.DataAnnotations.Schema;

namespace DeliveryBro.Areas.store.DTO
{
	public class HisOrderDTO
	{
			public int OrderId { get; set; }
			public string OrderDate { get; set; }
			public string CompletedTime { get; set; }
			public string CustomerName { get; set; }
			public Guid? CustomerId { get; set; }
			public string Note { get; set; }
			public int Total { get; set; }
			public ICollection<OrderDetailsDTO> OrderDetails { get; set; }
    }
}
