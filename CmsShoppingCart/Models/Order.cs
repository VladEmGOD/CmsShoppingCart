using System;
using System.Collections.Generic;

namespace CmsShoppingCart.WebApp.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        
        public string UserId { get; set; }
        
        public string Data { get; set; } 
    }

    public class OrderData
    {
        public List<OrderItem> Items { get; set; }

        public decimal GrandTotal{ get; set; }
    }

    public class OrderViewModel 
    {
        public List<OrderData> Items { get; set; }

        public decimal GrandTotal { get; set; }

    }

    public class OrdersViewModel
    {
        public List<OrderViewModel> Items { get; set; } = new();

        public class OrderViewModel
        {
            public Guid Id { get; set; }

            public decimal GrandTotal { get; set; }

        }
    }
}
