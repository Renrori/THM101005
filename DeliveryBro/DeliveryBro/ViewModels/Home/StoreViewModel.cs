namespace DeliveryBro.ViewModels.Home
{
    public class StoreViewModel
    {
        public Guid StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public string StoreDescription { get; set; }
        public LocationViewModel Location { get; set; }
    }
}
