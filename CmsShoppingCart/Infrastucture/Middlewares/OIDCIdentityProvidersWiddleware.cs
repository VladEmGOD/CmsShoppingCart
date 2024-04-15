using Microsoft.AspNetCore.Http;
using CmsShoppingCart.WebApp.Infrastucture;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using CmsShoppingCart.WebApp.Infrastucture.Services;

namespace CmsShoppingCart.WebApp.Infrastucture.Middlewares
{
    public class OIDCIdentityProvidersWiddleware(
        IAuthenticationSchemeProvider schemeProvider,
        IIdentityProvidersManager identityProvidersManager,
        CmsShoppingCartContext db) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var identityProviders = await db.SSOIdentityProviders.ToListAsync();
            var schemas = await schemeProvider.GetAllSchemesAsync();

            foreach (var provider in identityProviders)
            {
                if (schemas.Any(s => s.Name == provider.Name))
                    continue;

                await identityProvidersManager.AddProvider(provider);
            }

            await next(context);
        }
    }
}
