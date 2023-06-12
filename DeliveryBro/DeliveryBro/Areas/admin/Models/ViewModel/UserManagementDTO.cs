namespace DeliveryBro.Areas.admin.Models.ViewModel
{
    public class UserManagementDTO
    {
        public int Count { get; set; }
        public List<UserDTO>Items { get; set; }
    }
    
    public class UserDTO{
        public string CustomerAccount { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerPassword { get; set; }

    }
}
