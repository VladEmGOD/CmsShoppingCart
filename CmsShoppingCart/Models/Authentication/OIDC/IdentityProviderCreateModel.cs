using CmsShoppingCart.WebApp.Infrastucture;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace CmsShoppingCart.WebApp.Models.Authentication.OIDC;

public class IdentityProviderCreateInput
{
    public string Name { get; set; }

    public string ApplicationId { get; set; }

    public string ClientSecret { get; set; }

    public string Url { get; set; }

    [FileExtension]
    public IFormFile ImageUpload { get; set; }

    public IdentityProvider ToModel() => new()
    {
        Name = Name,
        ApplicationId = ApplicationId,
        Url = Url,
        Image = ImageUpload.FileName
    };
}

