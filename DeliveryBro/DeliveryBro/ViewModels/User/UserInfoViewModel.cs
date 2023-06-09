﻿namespace DeliveryBro.ViewModels.User
{
    public class UserInfoViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set;}
        public string? UserEmail { get; set;}
        public ICollection<string> UserAddress { get; set;}
        public DateTime? UserBirth {get; set;}
        public string? UserPhone { get; set;}
        public byte[]? UserPicture { get; set;}

    }
}
