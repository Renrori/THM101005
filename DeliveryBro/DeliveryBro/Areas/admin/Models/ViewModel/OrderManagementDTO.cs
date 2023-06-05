namespace DeliveryBro.Areas.admin.Models.ViewModel
{
    public class OrderManagementDTO
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerAddress { get; set; }
        public int AmountAfterDiscount { get; set; }
        public string OrderStatus { get; set; }

    }
}
