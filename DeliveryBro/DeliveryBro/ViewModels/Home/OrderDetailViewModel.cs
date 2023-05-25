namespace DeliveryBro.ViewModels.Home
{
    public class OrderDetailViewModel
    {
        public int OrderId { get; set; }
        public int DishId { get; set; }
        public DateTime OrderDate { get; set; }
        public int UnitPrice { get; set; }
        public int Quantity { get; set; }

        public int Subtotal { get; set; }
        public int Total { get; set; }
        
        public string DishName { get; set; }
    }
}

