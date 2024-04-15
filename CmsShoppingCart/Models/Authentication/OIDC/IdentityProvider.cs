using System;

namespace CmsShoppingCart.WebApp.Models.Authentication.OIDC;

public class IdentityProvider
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string ApplicationId { get; set; }

    public string Url { get; set; }

    public string ClientSecret { get; set; }

    public string Image { get; set; }
}
