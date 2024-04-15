using CmsShoppingCart.WebApp.Models.Authentication.OIDC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Models.Authentication
{
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password), Required, MinLength(4, ErrorMessage = "Minimum lenght is 4")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public IEnumerable<IdentityProviderViewModel> IdentityProviders { get; set; } = new List<IdentityProviderViewModel>();
    }
}
