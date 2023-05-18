﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DeliveryBro.Models
{
    public partial class RestaurantTable
    {
        public RestaurantTable()
        {
            CustomerOrderTable = new HashSet<CustomerOrderTable>();
            MenuTable = new HashSet<MenuTable>();
        }

        public int RestaurantId { get; set; }
        public string RestaurantAccount { get; set; }
        public string RestaurantPassword { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantAddress { get; set; }
        public string RestaurantPhone { get; set; }
        public string RestaurantEmail { get; set; }
        public TimeSpan? OpeningHours { get; set; }
        public string RestaurantDescription { get; set; }
        public byte[] RestaurantPicture { get; set; }

        public virtual ICollection<CustomerOrderTable> CustomerOrderTable { get; set; }
        public virtual ICollection<MenuTable> MenuTable { get; set; }
    }
}