﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace sv.Sale.DBModels
{
    public partial class ProductImage
    {
        public string ImageId { get; set; }
        public string ProductId { get; set; }
        public string ImageLink { get; set; }

        public virtual Product Product { get; set; }
    }
}