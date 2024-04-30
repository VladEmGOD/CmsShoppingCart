using CmsShoppingCart.WebApp.Infrastucture;
using CmsShoppingCart.WebApp.Models;
using CmsShoppingCart.WebApp.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Controllers
{
    [Authorize]
    public class OrdersController(UserManager<AppUser> userManeger, CmsShoppingCartContext db) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var id = await GetUserId();

            var orders = await db.Orders.Where(o => o.UserId == id).ToListAsync();
            var ordersViewModel = new OrdersViewModel();

            foreach (var order in orders)
            {
                var data = JsonSerializer.Deserialize<OrderData>(order.Data);
                ordersViewModel.Items.Add(
                    new()
                    {
                        Id = order.Id,
                        GrandTotal = data.GrandTotal,
                    });
            }

            return View(ordersViewModel);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var order = await db.Orders.Where(o => o.Id == id).FirstOrDefaultAsync();

            if (order is null)
                return NotFound();

            var data = JsonSerializer.Deserialize<OrderData>(order.Data);

            return View(data);
        }

        private async Task<string> GetUserId()
        {
            var authClaim = User.Claims.FirstOrDefault(c => c.Type == ShopClaimTypes.AuthenticationScheme);

            if (authClaim is null)
            {
                var appUser = await userManeger.FindByNameAsync(User.Identity.Name);
                return appUser?.Id;
            }

            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}
