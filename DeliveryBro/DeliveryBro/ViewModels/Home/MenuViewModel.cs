namespace DeliveryBro.ViewModels.Home
{
    public class MenuViewModel
    {
        public int DishId { get; set; }
        public int RestaurantId { get; set; }
        public string DishName { get; set; }
        public int DishPrice { get; set; }
        public byte[] DishPicture { get; set; }
        public string DishDescription { get; set; }
        public string DishCategory { get; set; }

    }
}
