using DeliveryBro.Areas.store.DTO;

namespace DeliveryBro.ViewModels.User
{
    public class UserOrderViewModel
    {
        public int OrderId { get; set; }
        public string OrderDate { get; set; }
        //public string DishName { get; set; }
        public string CustomerName { get; set; }
        public string Note { get; set; }
        public int Total { get; set; }
        public ICollection<UserOrderDetailsViewModel> OrderDetails { get; set; }
    }
}
