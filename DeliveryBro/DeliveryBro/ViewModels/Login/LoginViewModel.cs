//using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace DeliveryBro.ViewModels.Login
{
    public class LoginViewModel : IValidatableObject
    {
        [Required(ErrorMessage ="請輸入帳號")]
        [StringLength(maximumLength:15,MinimumLength =4 , ErrorMessage ="帳號最少需要4個字符")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "帳號只能使用英文和數字")]
        [Display(Name ="帳號")]
        public string UserAccount { get; set; }

        
        [StringLength(maximumLength: 15, MinimumLength = 4, ErrorMessage = "密碼不能少於4個字符")]
        [Required(ErrorMessage = "請輸入密碼")]
        [Display(Name ="密碼")]
        public string UserPassword { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(UserAccount) && string.IsNullOrEmpty(UserPassword))
            {
                yield return new ValidationResult(errorMessage: "請填入使用者資訊!", new string[] { "UserAccount", "UserPassword" });
            }
        }
    }
}
