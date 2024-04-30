using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Models
{
    public class OrderItem
    {
        public int ProductId { get; set; }
        public string PriductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get { return Quantity * Price; } }
        public string Image { get; set; }

        public OrderItem()
        {
                
        }

        public OrderItem(CartItem cartItem)
        {
            ProductId = cartItem.ProductId;
            PriductName = cartItem.PriductName;
            Price = cartItem.Price;
            Quantity = cartItem.Quantity;
            Image = cartItem.Image;
        }
    }
}
