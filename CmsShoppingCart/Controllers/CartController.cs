using CmsShoppingCart.Infrastucture;
using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.Controllers
{
    public class CartController : Controller
    {
        private readonly CmsShoppingCartContext _context;

        public CartController(CmsShoppingCartContext context)
        {
            _context = context;
        }

        //GET /cart
        public IActionResult Index()
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            CartViewModel cardVM = new CartViewModel
            {
                CartItems = cart,
                GrandTotal = cart.Sum(x => x.Price * x.Quantity)
            };

            return View(cardVM);
        }

        //GET /cart/add/Id
        
        public async Task<IActionResult> Add(int id)
        {
            Product product = await _context.Products.FindAsync(id);

            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            CartItem cartItem = cart.Where(x => x.ProductId == id).FirstOrDefault();

            if (cartItem == null)
            {
                cart.Add(new CartItem(product));
            }
            else 
            {
                cartItem.Quantity += 1;
            }

            HttpContext.Session.SetJson("Cart", cart);

            return RedirectToAction("Index");
        }

        //GET /cart/decrease/Id
        public IActionResult Decrease(int id)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

            CartItem cartItem = cart.Where(x => x.ProductId == id).FirstOrDefault();

            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(x => x.ProductId == id);
            }

            HttpContext.Session.SetJson("Cart", cart);

            if (cart.Count == 0) 
            {
                HttpContext.Session.Remove("Cart");
            }

            return RedirectToAction("Index");
        }

        //GET /cart/remove/Id
        public IActionResult Remove(int id)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

            cart.RemoveAll(x => x.ProductId == id); 

            HttpContext.Session.SetJson("Cart", cart);

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }

            return RedirectToAction("Index");
        }

        //GET /cart/clear
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Index");
        }

    }
}
