using System.ComponentModel.DataAnnotations;


namespace DeliveryBro.Areas.store.ViewModels
{
    public class RegisterViewModel
    {
		[Required(ErrorMessage = "帳號為必填欄位!")]
		[StringLength(10, ErrorMessage = "長度不能超過10個字元")]
		[Display(Name = "帳號")]
		public string RestaurantAccount { get; set; }

		//[Required(ErrorMessage ="密碼為必填欄位!")]
		//[StringLength(maximumLength: 15, MinimumLength = 4, ErrorMessage = "密碼最少需要4個字元")]
		//[RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "密碼只能使用英文和數字")]
		[Display(Name = "密碼")]
		public string RestaurantPassword { get; set; }

		//[Required(ErrorMessage ="密碼為必填欄位!")]
		//[StringLength(maximumLength: 15, MinimumLength = 4, ErrorMessage = "密碼最少需要4個字元")]
		//[RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "確認密碼與密碼不一致")]
		[Display(Name = "確認密碼")]
		public string ConfirmRestaurantPassword { get; set; }

		//[Required(ErrorMessage = "餐廳名稱為必填欄位!")]
		//[StringLength(20, ErrorMessage = "長度不能超過20個字元")]
		//[Display(Name = "餐廳名稱")]
		public string RestaurantName { get; set; }

		//[Required(ErrorMessage = "餐廳地址為必填欄位!")]
		//[StringLength(30, ErrorMessage = "長度不能超過30個字元")]
		//[Display(Name = "餐廳地址")]
		public string RestaurantAddress { get; set; }

		//[Required(ErrorMessage = "餐廳電話為必填欄位!")]
		//[StringLength(10, ErrorMessage = "長度不能超過10個字元")]
		//[Display(Name = "餐廳電話")]
		public string RestaurantPhone { get; set; }

		//[Required(ErrorMessage = "餐廳信箱為必填欄位!")]
		//[StringLength(30, ErrorMessage = "長度不能超過30個字元")]
		//[Display(Name = "餐廳信箱")]
		public string RestaurantEmail { get; set; }
		//public TimeSpan OpeningHours { get; set; }

		//public string RestaurantDescription { get; set;}

		//public byte[] RestaurantPicture { get; set; }



	}
}
