using CmsShoppingCart.WebApp.Infrastucture;
using CmsShoppingCart.WebApp.Models;
using CmsShoppingCart.WebApp.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Controllers
{
    public class CartController(CmsShoppingCartContext db, UserManager<AppUser> userManeger) : Controller
    {
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
            Product product = await db.Products.FindAsync(id);

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

            if (HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
            {
                return RedirectToAction("Index");
            }

            return ViewComponent("SmallCart");
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

            //return RedirectToAction("Page", "Pages");
            //return Redirect("/");
            return Redirect(Request.Headers["Referer"].ToString());
        }

        //GET /cart/clear
        public async Task<IActionResult> Chechout()
        {
            var cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var orderItems = new List<OrderItem>();

            foreach (var item in cart) 
            {
                orderItems.Add(new OrderItem(item));
            }

            var orderData = new OrderData();
            orderData.Items = orderItems;
            orderData.GrandTotal = orderItems.Sum(x => x.Price * x.Quantity);

            var data = JsonSerializer.Serialize(orderData);

            var order = new Order();
            order.Id = new Guid();
            order.Data = data;

            var authClaim = User.Claims.FirstOrDefault(c => c.Type == ShopClaimTypes.AuthenticationScheme);

            if (authClaim is null)
            {
                var appUser = await userManeger.FindByNameAsync(User.Identity.Name);
                order.UserId = appUser.Id;
            }
            else 
            {
                var sid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                order.UserId = sid.Value;
            }

            db.Orders.Add(order);
            await db.SaveChangesAsync();
            return this.Clear();
        }
    }
}
