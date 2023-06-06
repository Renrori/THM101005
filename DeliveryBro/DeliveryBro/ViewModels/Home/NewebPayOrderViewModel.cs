namespace DeliveryBro.ViewModels.Home
{
    public class NewebPayOrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerAddress { get; set; }
        public int? ShippingFee { get; set; }
        public string Payment { get; set; }
        public string Email { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }

        public string Note { get; set; }

        public Guid CustomerId { get; set; }
        public Guid RestaurantId { get; set; }
        public List<OrderDetailViewModel> OrderDetailViewModels { get; set; }

    }
}
