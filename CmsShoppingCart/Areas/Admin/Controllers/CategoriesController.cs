﻿using CmsShoppingCart.WebApp.Infrastucture;
using CmsShoppingCart.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly CmsShoppingCartContext _context;

        public CategoriesController(CmsShoppingCartContext context)
        {
            _context = context;
        }

        //Get: /admon/categories/
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.OrderBy(x => x.Sorting).ToListAsync());
        }

        //Get: /admon/categories/create
        public IActionResult Create() => View();

        //POST: /admon/categories/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.ToLower().Replace(" ", "-");
                category.Sorting = 100;

                var slug = await _context.Categories.FirstOrDefaultAsync(x => x.Slug == category.Slug);

                if (slug != null)
                {
                    ModelState.AddModelError("", "The category already exists!");
                    return View(category);
                }

                _context.Add(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "The category has been created";

                return RedirectToAction("index");
            }

            return View(category);

        }
        //POST: /admon/categories/edit
        public async Task<IActionResult> Edit(int id)
        {
            Category category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        //POST: /admon/categories/edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.ToLower().Replace(" ", "-");

                var slug = await _context.Categories
                    .Where(x => x.Id != category.Id)
                    .FirstOrDefaultAsync(x => x.Slug == category.Slug);

                if (slug != null)
                {
                    ModelState.AddModelError("", "The category already exists!");
                    return View(category);
                }

                _context.Update(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "The category has been updated";

                return RedirectToAction("Edit", new { id = category.Id });
            }

            return View(category);
        }

        //GET admin/pages/delete/X
        public async Task<IActionResult> Delete(int id)
        {
            Category category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                TempData["Error"] = "The category does not exist";

            }
            else
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "The category has been deleted";
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Reorder(int[] id)
        {
            int count = 1;
            foreach (var categoryId in id)
            {
                Category category = await _context.Categories.FindAsync(categoryId);
                category.Sorting = count;
                _context.Update(category);
                await _context.SaveChangesAsync();
                count++;
            }
            return Ok();
        }
    }
}
