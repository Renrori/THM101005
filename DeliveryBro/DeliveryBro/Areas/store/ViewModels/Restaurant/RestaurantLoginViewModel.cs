using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DeliveryBro.Areas.store.ViewModels.Restaurant
{
    public class RestaurantLoginViewModel
    {
        [Required(ErrorMessage = "帳號為必填欄位")]
        //[StringLength(maximumLength: 8, MinimumLength = 3, ErrorMessage = "帳號必須介於3~8個字元")]
        [Display(Name = "帳號")]
        public string RestaurantAccount { get; set; }

        [Required(ErrorMessage = "密碼為必填欄位")]
        //[Display(Name = "密碼")]
        public string RestaurantPassword { get; set; }


    }
}
