﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DeliveryBro.Models
{
    public partial class DriverTable
    {
        public DriverTable()
        {
            CustomerOrderTable = new HashSet<CustomerOrderTable>();
        }

        public string DriverAccount { get; set; }
        public string DriverPassword { get; set; }
        public string DriverName { get; set; }
        public string DriverEmail { get; set; }
        public string DriverPhone { get; set; }
        public int? DriveLicense { get; set; }
        public byte[] DriverPicture { get; set; }
        public Guid DriverId { get; set; }

        public virtual ICollection<CustomerOrderTable> CustomerOrderTable { get; set; }
    }
}