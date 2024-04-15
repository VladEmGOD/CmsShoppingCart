using CmsShoppingCart.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> userManeger;

        public UsersController(UserManager<AppUser> userManeger)
        {
            this.userManeger = userManeger;
          
        }

        public IActionResult Index()
        {
            return View(userManeger.Users);
        }
    }
}
