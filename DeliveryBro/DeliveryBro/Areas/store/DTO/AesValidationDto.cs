namespace DeliveryBro.Areas.store.DTO
{
    public class AesValidationDto
    {
        public AesValidationDto(string RestaurantEmail, DateTime ExpiredDate)
        {
            this.RestaurantEmail = RestaurantEmail;
            this.ExpiredDate = ExpiredDate;
        }
        public string RestaurantEmail { get; set; }
        public DateTime ExpiredDate { get; set; }//結束日期
    }

}
