namespace DeliveryBro.Areas.store.DTO
{
    public class MenuDTO
    {
            public int DishId { get; set; }
            public string DishName { get; set; }
            public int DishPrice { get; set; }
            public string? DishDescription { get; set; }
            public string DishCategory { get; set; }
            public string? DishStatus { get; set; }
            public int? RestaurantId { get; set; }
            public string? PictureUrl { get; set; }

	}
}
