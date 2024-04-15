using CmsShoppingCart.WebApp.Infrastucture;
using Microsoft.AspNetCore.Http;
using System;

namespace CmsShoppingCart.WebApp.Models.Authentication.OIDC;

public class IdentityProviderEditInput
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string ApplicationId { get; set; }

    public string Url { get; set; }

    public string ClientSecret { get; set; }

    public string Image { get; set; }

    [FileExtension]
    public IFormFile ImageUpload { get; set; }

    public IdentityProviderEditInput() { }

    public IdentityProviderEditInput(IdentityProvider provider)
    {
        Id = provider.Id;
        Name = provider.Name;
        ApplicationId = provider.ApplicationId;
        Url = provider.Url;
        ClientSecret = provider.ClientSecret;
        Image = provider.Image;
    }

    public IdentityProvider ToModel() => new()
    {
        Id = Id,
        Name = Name,
        ApplicationId = ApplicationId,
        Url = Url,
        ClientSecret = ClientSecret,
        Image = Image
    };
}

