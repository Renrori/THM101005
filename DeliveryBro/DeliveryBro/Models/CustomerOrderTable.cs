﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DeliveryBro.Models
{
    public partial class CustomerOrderTable
    {
        public int OrderId { get; set; }
        public string CustomerAddress { get; set; }
        public int? ShippingFee { get; set; }
        public string Payment { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public int? CouponId { get; set; }
        public float Discount { get; set; }
        public int AmountAfterDiscount { get; set; }
        public string Note { get; set; }
        public int? DriverId { get; set; }
        public int CustomerId { get; set; }
        public int RestaurantId { get; set; }

        public virtual CustomersTable Customer { get; set; }
        public virtual DriverTable Driver { get; set; }
        public virtual RestaurantTable Restaurant { get; set; }
        public virtual OrderDetailsTable OrderDetailsTable { get; set; }
    }
}