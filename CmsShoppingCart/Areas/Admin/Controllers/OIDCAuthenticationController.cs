using CmsShoppingCart.WebApp.Models.Authentication.OIDC;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using CmsShoppingCart.WebApp.Infrastucture;
using CmsShoppingCart.WebApp.Infrastucture.Providers;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace CmsShoppingCart.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
public class OIDCAuthenticationController(
    IAuthenticationSchemeProvider schemeProvider, 
    IOptionsMonitorCache<OpenIdConnectOptions> optionsCache, 
    CmsShoppingCartContext db, 
    IMediaImageProvider imageProvider, 
    OpenIdConnectPostConfigureOptions postConfigureOptions) : Controller
{
    private string ImagesCatalogName = "sso-icons";

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var providers = await db.SSOIdentityProviders.ToListAsync();
        return View(providers);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Remove(string scheme)
    {
        schemeProvider.RemoveScheme(scheme);
        optionsCache.TryRemove(scheme);
        return Redirect("/");
    }

    public async Task<IActionResult> Create(IdentityProviderCreateInput input)
    {
        if (!ModelState.IsValid)
            return View(input);

        string imageName = "noimage.png";

        if (input.ImageUpload != null)
        {
            imageName = await imageProvider.SaveAsync(input.ImageUpload, ImagesCatalogName);
        }

        var provider = input.ToModel();
        provider.Image = imageName;

        db.Update(provider);
        await db.SaveChangesAsync();
        
        var newProviderId = provider.Id.ToString();

        if (await schemeProvider.GetSchemeAsync(newProviderId) == null)
        {
            schemeProvider.AddScheme(new AuthenticationScheme(newProviderId, provider.Name, typeof(OpenIdConnectHandler)));
        }
        else
        {
            optionsCache.TryRemove(newProviderId);
        }

        var o = new OpenIdConnectOptions();
        o.Authority = provider.Url;
        o.ClientId = provider.ApplicationId;
        o.ClientSecret = provider.ClientSecret;
        o.MapInboundClaims = false;
        //o.Configuration = new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration()
        //{

        //};
        o.Scope.Add("email");

        postConfigureOptions.PostConfigure(newProviderId, o);
        optionsCache.TryAdd(newProviderId, o);

        TempData["Success"] = "The provider has been created";

        return RedirectToAction("index");
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var provider = await db.SSOIdentityProviders.FirstOrDefaultAsync(p => p.Id == id);

        if (provider == null)
            return NotFound();

        return View(new IdentityProviderEditInput(provider));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(IdentityProviderEditInput input)
    {
        if (!ModelState.IsValid)
            return View(input);

        if (input.ImageUpload != null && input.Image != "noimage.png")
        {
            var newImageName = await imageProvider.AddOrUpdateAsync(input.Image, input.ImageUpload, ImagesCatalogName);
            input.Image = newImageName;
        }

        var provider = input.ToModel();
        db.SSOIdentityProviders.Update(provider);
        await db.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var provider = await db.SSOIdentityProviders.FindAsync(id);

        if (provider == null)
        {
            TempData["Error"] = "The provider does not exist";
            return StatusCode(500);
        }

        if (!string.Equals(provider.Image, "noimage.png"))
        {
            imageProvider.Delete(provider.Image, ImagesCatalogName);
        }

        db.SSOIdentityProviders.Remove(provider);
        await db.SaveChangesAsync();

        TempData["Success"] = "The provider has been deleted";

        return RedirectToAction("Index");
    }
}
