namespace DeliveryBro.Areas.store.ViewModels
{
    public class RegisterViewModel
    {
        public string RestaurantAccount { get; set; }
        public string RestaurantPassword { get; set; }
        public string RestaurantName { get; set; }
		public string RestaurantAddress { get; set; }
		public string RestaurantPhone { get; set; }
		public string RestaurantEmail { get; set; }
		public string OpeningHours { get; set; }

		public string RestaurantDescription { get; set;}

		public byte[] RestaurantPicture { get; set; }



	}
}
