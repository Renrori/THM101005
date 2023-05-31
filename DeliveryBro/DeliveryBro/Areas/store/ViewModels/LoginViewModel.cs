using System.ComponentModel.DataAnnotations;

namespace DeliveryBro.Areas.store.ViewModels
{
    public class LoginViewModel
    {
		[Required(ErrorMessage ="帳號為必填欄位!")]
		[Display(Name ="帳號")]
		
		public string Account { get; set; }
		[Required(ErrorMessage = "密碼為必填欄位!")]
		[Display(Name = "密碼")]
		public string Password { get; set; }
    }
}
