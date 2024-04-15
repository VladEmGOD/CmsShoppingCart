using CmsShoppingCart.WebApp.Infrastucture;
using CmsShoppingCart.WebApp.Infrastucture.Providers;
using CmsShoppingCart.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly CmsShoppingCartContext _context;

        private readonly IMediaImageProvider imageProvider;

        private string ImagesCatalogName = "products";
        private string DefaultImageName = "noimage.png";

        public ProductsController(CmsShoppingCartContext context, IMediaImageProvider imageProvider)
        {
            _context = context;
            this.imageProvider = imageProvider;
        }

        //Get: /admin/Products/
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

        //GET admin/products/details/X
        public async Task<IActionResult> Details(int id)
        {
            Product product = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }



        //GET admin/products/create/
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_context.Categories.OrderBy(x => x.Sorting), "Id", "Name");
            return View();
        }

        //POST admin/products/create/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.CategoryId = new SelectList(_context.Categories.OrderBy(x => x.Sorting), "Id", "Name");

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.ToLower().Replace(" ", "-");

                var slug = await _context.Products.FirstOrDefaultAsync(x => x.Slug == product.Slug);

                if (slug != null)
                {
                    ModelState.AddModelError("", "The product already exists!");
                    return View(product);
                }

                if (product.ImageUpload != null)
                    product.Image = await imageProvider.SaveAsync(product.ImageUpload, ImagesCatalogName);
                else 
                    product.Image = DefaultImageName;

                _context.Add(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = "The product has been created";

                return RedirectToAction("index");
            }

            return View(product);

        }

        //GET admin/products/edit/X
        public async Task<IActionResult> Edit(int id)
        {
            Product product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.CategoryId = new SelectList(_context.Categories.OrderBy(x => x.Sorting), "Id", "Name", product.CategoryId);
            return View(product);
        }

        //POST admin/products/Edit/Id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            ViewBag.CategoryId = new SelectList(_context.Categories.OrderBy(x => x.Sorting), "Id", "Name", product.CategoryId);

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.ToLower().Replace(" ", "-");

                var slug = await _context.Products.Where(X => X.Id != id).FirstOrDefaultAsync(x => x.Slug == product.Slug);

                if (slug != null)
                {
                    ModelState.AddModelError("", "The product already exists!");
                    return View(product);
                }

                if (product.ImageUpload != null && product.Image != DefaultImageName)
                {
                    var image = await imageProvider.AddOrUpdateAsync(product.Image, product.ImageUpload, ImagesCatalogName);
                    product.Image = image;
                }

                _context.Update(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = "The product has been edited";
                return RedirectToAction("index");
            }

            return View(product);

        }

        //GET admin/product/delete/X
        public async Task<IActionResult> Delete(int id)
        {
            Product product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                TempData["Error"] = "The product does not exist";
            }
            else
            {
                if (!string.Equals(product.Image, DefaultImageName))
                {
                    imageProvider.Delete(product.Image, ImagesCatalogName);
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "The product has been deleted";
            }

            return RedirectToAction("Index");
        }


    }
}
