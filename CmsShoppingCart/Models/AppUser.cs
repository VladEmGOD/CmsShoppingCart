using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Models
{
    public class AppUser : IdentityUser
    {
        public string Ocupation { get; set; }

        public AuthenticationType AuthenticationType { get; set; }

        public string IdentityProviderId  { get; set; }
    }

    public enum AuthenticationType 
    { 
        Internal = 0,
        External = 1
    }
}
