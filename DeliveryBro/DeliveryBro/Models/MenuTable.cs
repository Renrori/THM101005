﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DeliveryBro.Models
{
    public partial class MenuTable
    {
        public int DishId { get; set; }
        public string DishName { get; set; }
        public int DishPrice { get; set; }
        public string DishDescription { get; set; }
        public string DishCategory { get; set; }
        public byte[] DishPicture { get; set; }
        public string DishStatus { get; set; }
        public Guid RestaurantId { get; set; }
        public string PicturePath { get; set; }

        public virtual RestaurantTable Restaurant { get; set; }
    }
}