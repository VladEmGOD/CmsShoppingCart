using System;

namespace CmsShoppingCart.WebApp.Models.Authentication.OIDC
{
    public class IdentityProviderViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public IdentityProviderViewModel() { }

        public IdentityProviderViewModel(IdentityProvider provider)
        {
            Id = provider.Id;
            Image = provider.Image;
            Name = provider.Name;
        }
    }
}
