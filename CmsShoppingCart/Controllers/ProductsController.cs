﻿using CmsShoppingCart.WebApp.Infrastucture;
using CmsShoppingCart.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly CmsShoppingCartContext _context;

        public ProductsController(CmsShoppingCartContext context)
        {
            _context = context;
        }

        //GET /products
        public async Task<IActionResult> Index(int p = 1)
        {
            int pageSize = 6;

            var products = _context.Products.OrderByDescending(x => x.Id)
                 .Include(x => x.Category).Skip((p - 1) * pageSize)
                .Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotlaPages = (int)Math.Ceiling((decimal)_context.Products.Count() / pageSize);

            return View(await products.ToListAsync());
        }

        //GET /products/category
        public async Task<IActionResult> ProductsByCategory(string slug, int p = 1)
        {
            Category category = _context.Categories.Where(x => x.Slug == slug).FirstOrDefault();

            if (category == null)
            {
                return RedirectToAction("Index");
            }

            int pageSize = 6;

            var products = _context.Products.OrderByDescending(x => x.Id)
                 .Include(x => x.Category).Skip((p - 1) * pageSize)
                .Where(x => x.CategoryId == category.Id)
                .Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotlaPages = (int)Math.Ceiling((decimal)_context.Products.Where(x => x.CategoryId == category.Id).Count() / pageSize);
            ViewBag.CategoryName = category.Name;
            ViewBag.CategorySlug = category.Slug;

            return View(await products.ToListAsync());
        }
    }
}
