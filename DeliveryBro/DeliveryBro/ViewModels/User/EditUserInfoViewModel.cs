namespace DeliveryBro.ViewModels.User
{
    public class EditUserInfoViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string? UserEmail { get; set; }
        public DateTime? UserBirth { get; set; }
        public string? UserPhone { get; set; }
        public byte[]? UserPicture { get; set; }
    }
}
