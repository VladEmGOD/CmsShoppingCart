using CmsShoppingCart.WebApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CmsShoppingCart.WebApp.Models.Authentication.OIDC;

namespace CmsShoppingCart.WebApp.Infrastucture
{
    public class CmsShoppingCartContext : IdentityDbContext<AppUser>
    {
        public CmsShoppingCartContext(DbContextOptions<CmsShoppingCartContext> options)
            :base(options)
        {
        }

        public DbSet<Page> Pages { get; set; }
        
        public DbSet<Category> Categories { get; set; }
        
        public DbSet<Product> Products { get; set; }

        public DbSet<IdentityProvider> SSOIdentityProviders { get; set; }
    }
}
