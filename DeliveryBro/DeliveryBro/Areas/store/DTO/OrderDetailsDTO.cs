namespace DeliveryBro.Areas.store.DTO
{
    public class OrderDetailsDTO
    {
        //public int OrderId { get; set; }
        //public DateTime OrderDate { get; set; }
        public string DishName { get; set; }
        public int UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int? Discount { get; set; }
        public int Subtotal { get; set; }
        
    }
}