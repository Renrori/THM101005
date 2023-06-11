using DeliveryBro.ViewModels;

namespace DeliveryBro.Areas.deliver.DTO
{
    public class HistoryOrderDTO
    {
        public int OrderId { get; set; }
        public string OrderDate { get; set; }
        public string CustomerAddress { get;set; }
        public string RestaurantAddress { get; set; }
        public LocationViewModel CustomerLocation { get; set; }
        public LocationViewModel RestaurantLocation { get; set; }
		public string CompletedTime { get; set; }
        public string CustomerName { get; set; }
        public string RestaurantName { get; set; }
        public Guid? CustomerId { get; set; }
        public string Note { get; set; }
        public int Total { get; set; }
        public double RandCdistance { get; set; }
        public string OrderStatus { get; set; }
        public ICollection<OrderDetailsDTO> OrderDetails { get; set; }
    }
}
