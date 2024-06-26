﻿using CmsShoppingCart.WebApp.Infrastucture;
using CmsShoppingCart.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Editor")]
    [Area("Admin")]
    public class PagesController : Controller
    {
        private readonly CmsShoppingCartContext _context;

        public PagesController(CmsShoppingCartContext context)
        {
            _context = context;
        }
        
        //GET admin/pages
        public async Task<IActionResult> Index()
        {
            IQueryable<Page> pages = from p in _context.Pages orderby p.Sorting select p;

            List<Page> pagesList = await pages.ToListAsync();

            return View(pagesList);
        }

        //GET admin/pages/details/X
        public async Task<IActionResult> Details(int id)
        {
            Page page = await _context.Pages.FirstOrDefaultAsync(x => x.Id == id);

            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        //GET admin/pages/create/X
        public IActionResult Create() =>  View();


        //POST admin/pages/Create/X
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Page page)
        {
            if (ModelState.IsValid) 
            {
                page.Slug = page.Title.ToLower().Replace(" ", "-");
                page.Sorting = 100;

                var slug = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == page.Slug);

                if (slug != null) 
                {
                    ModelState.AddModelError("", "The page already exists!");
                    return View(page);
                }

                _context.Add(page);
                await _context.SaveChangesAsync();

                TempData["Success"] = "The page has been created";

                return RedirectToAction("index");
            }

            return View(page);

        }

        //GET admin/pages/edit/X
        public async Task<IActionResult> Edit(int id)
        {
            Page page = await _context.Pages.FindAsync(id);

            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Page page)
        {
            if (ModelState.IsValid)
            {
                page.Slug = page.Id == 1 ? "home" : page.Title.ToLower().Replace(" ", "-");

                var slug = await _context.Pages
                    .Where(x =>x.Id != page.Id)
                    .FirstOrDefaultAsync(x => x.Slug == page.Slug);

                if (slug != null)
                {
                    ModelState.AddModelError("", "The page already exists!");
                    return View(page);
                }

                _context.Update(page);
                await _context.SaveChangesAsync();

                TempData["Success"] = "The page has been created";

                return RedirectToAction("Edit", new {id = page.Id });
            }

            return View(page);
        }

        //GET admin/pages/delete/X
        public async Task<IActionResult> Delete(int id)
        {
            Page page = await _context.Pages.FindAsync(id);

            if (page == null)
            {
                TempData["Error"] = "The page does not exist";

            }
            else 
            {
                _context.Pages.Remove(page);
                await _context.SaveChangesAsync();
                TempData["Success"] = "The page has been deleted";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Reorder(int[] id)
        {
            int count = 1;
            foreach (var pageId in id)
            {
                Page page = await _context.Pages.FindAsync(pageId);
                page.Sorting = count;
                _context.Update(page);
                await _context.SaveChangesAsync();
                count++;
            }
            return Ok();
        }

    }
}
