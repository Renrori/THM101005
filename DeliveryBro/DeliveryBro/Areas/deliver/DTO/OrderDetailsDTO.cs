namespace DeliveryBro.Areas.deliver.DTO
{
    public class OrderDetailsDTO
    {
        public string DishName { get; set; }
        public int UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int? Discount { get; set; }
        public int Subtotal { get; set; }
    }
}
