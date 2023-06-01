namespace DeliveryBro.ViewModels
{
    public class AesValidationViewModel
    {
        public AesValidationViewModel(string Account, DateTime ExpiredDate)
        {
            this.Account = Account;
            this.ExpiredDate = ExpiredDate;
        }
        public string Account { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}
