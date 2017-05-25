﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoShop.Areas.Admin.Models.ViewModels.Shop
{
    public class OrdersForAdminVM
    {
        public int OrderNumber { get; set; }
        public string Username { get; set; }
        public decimal Total { get; set; }
        public Dictionary<string, int> ProductsAntQty { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}