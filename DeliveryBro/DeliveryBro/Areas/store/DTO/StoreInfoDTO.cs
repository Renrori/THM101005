namespace DeliveryBro.Areas.store.DTO
{
    public class StoreInfoDTO
    {
        public Guid RestaurantId { get; set; }
        public string? RestaurantName { get; set; }
        public string? RestaurantAddress { get; set; }
        public string? RestaurantPhone { get; set; }
        public string? RestaurantEmail { get; set; }
        public TimeSpan? OpeningHours { get; set; }
		public TimeSpan? EndHours { get; set; }
		public int? PrepareTime { get; set; }
		public string? RestaurantDescription { get; set; }
        public byte[]? RestaurantPicture { get; set; }
        public string? RestaurantStatus { get; set; }
    }
}
