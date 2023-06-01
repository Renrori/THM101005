using System.ComponentModel.DataAnnotations;


namespace DeliveryBro.Areas.store.ViewModels
{
    public class RegisterViewModel
    {
		[Required(ErrorMessage = "帳號為必填欄位!")]
		[StringLength(10, ErrorMessage = "長度不能超過10個字元")]
		[Display(Name = "帳號")]
		public string RestaurantAccount { get; set; }
        public string RestaurantPassword { get; set; }
		public string ConfirmRestaurantPassword { get; set; }
		public string RestaurantName { get; set; }
		public string RestaurantAddress { get; set; }
		public string RestaurantPhone { get; set; }
		public string RestaurantEmail { get; set; }
		//public TimeSpan OpeningHours { get; set; }

		//public string RestaurantDescription { get; set;}

		//public byte[] RestaurantPicture { get; set; }



	}
}
