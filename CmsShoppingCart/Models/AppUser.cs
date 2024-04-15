using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Models
{
    public class AppUser : IdentityUser
    {
        public string Ocupation { get; set; }
    }
}
