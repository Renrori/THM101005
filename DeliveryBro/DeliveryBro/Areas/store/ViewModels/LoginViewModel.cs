using System.ComponentModel.DataAnnotations;

namespace DeliveryBro.Areas.store.ViewModels
{
    public class LoginViewModel 
	{
		//[Required(ErrorMessage ="帳號為必填欄位!")]
		//[StringLength(maximumLength: 15, MinimumLength = 4, ErrorMessage = "帳號最少需要4個字元")]
		//[RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "帳號只能使用英文和數字")]
		[Display(Name ="帳號")]
		
		public string? Account { get; set; }


		//[Required(ErrorMessage = "密碼為必填欄位!")]
		//[StringLength(maximumLength: 15, MinimumLength = 4, ErrorMessage = "密碼不能少於4個字元")]
		[Display(Name = "密碼")]
		public string? Password { get; set; }


		//public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		//{
		//	if (string.IsNullOrEmpty(Account) && string.IsNullOrEmpty(Password))
		//	{
		//		yield return new ValidationResult(errorMessage: "請填入使用者資訊!", new string[] { "Account", "Password" });
		//	}
		//}
	}
}
