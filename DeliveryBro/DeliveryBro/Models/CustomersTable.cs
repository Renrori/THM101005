﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DeliveryBro.Models
{
    public partial class CustomersTable
    {
        public CustomersTable()
        {
            CustomerAddressTable = new HashSet<CustomerAddressTable>();
            CustomerOrderTable = new HashSet<CustomerOrderTable>();
        }

        public string CustomerAccount { get; set; }
        public string CustomerPassword { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string CustomerPhone { get; set; }
        public byte[] CustomerPhoto { get; set; }
        public int? CouponId { get; set; }
        public string CustomerOauth { get; set; }
        public Guid CustomerId { get; set; }

        public virtual ICollection<CustomerAddressTable> CustomerAddressTable { get; set; }
        public virtual ICollection<CustomerOrderTable> CustomerOrderTable { get; set; }
    }
}