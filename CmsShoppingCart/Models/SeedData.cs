using CmsShoppingCart.WebApp.Infrastucture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Models
{
    public class SeedData
    {
        public static void initialize(IServiceProvider serviceProvider) 
        {
            using (CmsShoppingCartContext context = new CmsShoppingCartContext
                (serviceProvider.GetRequiredService<DbContextOptions<CmsShoppingCartContext>>())) 
            {
                if (context.Pages.Any()) 
                {
                    return;
                }

                context.Pages.AddRange(
                    new Page 
                    { 
                        Title = "Home",
                        Slug = "home",
                        Contetnt = "home page",
                        Sorting = 0
                    },  
                    new Page 
                    { 
                        Title = "About Us",
                        Slug = "about-us",
                        Contetnt = "about us page",
                        Sorting = 100
                    }, 
                    new Page 
                    { 
                        Title = "Services",
                        Slug = "services",
                        Contetnt = "services page",
                        Sorting = 100
                    }, 
                    new Page 
                    { 
                        Title = "Contact",
                        Slug = "contact",
                        Contetnt = "Contact page",
                        Sorting = 100
                    }
               );

                context.SaveChanges();
            }
        }
    }
}
