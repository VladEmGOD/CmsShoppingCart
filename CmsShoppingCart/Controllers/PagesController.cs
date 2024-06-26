﻿using CmsShoppingCart.WebApp.Infrastucture;
using CmsShoppingCart.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Controllers
{
    public class PagesController : Controller
    {
        private readonly CmsShoppingCartContext _context;

        public PagesController(CmsShoppingCartContext context)
        {
            _context = context;
        }

        //GET / of /slug
        public async Task<IActionResult> Page(string slug)
        {
            if (slug == null) return View(await _context.Pages.Where(x => x.Slug == "home").FirstOrDefaultAsync());

            Page page = await _context.Pages.Where(x => x.Slug == slug).FirstOrDefaultAsync(); 

            if (page == null) return NotFound();

            return View(page);
        }
    }
}
