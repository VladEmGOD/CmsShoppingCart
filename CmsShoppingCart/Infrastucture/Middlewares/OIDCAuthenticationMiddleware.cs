using Microsoft.AspNetCore.Http;
using CmsShoppingCart.WebApp.Infrastucture;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using CmsShoppingCart.WebApp.Infrastucture.Services;
using Auth0.AspNetCore.Authentication;

namespace CmsShoppingCart.WebApp.Infrastucture.Middlewares
{
    public class OIDCAuthenticationMiddleware() : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            //var internalAuthentication = await context.AuthenticateAsync();

            //if (internalAuthentication.Succeeded)
            //{
            //}

            //var externalAuthentication = await context.AuthenticateAsync(Auth0Constants.AuthenticationScheme);
            //if (externalAuthentication.Succeeded)
            //{
            //    context.User = externalAuthentication.Principal;

            //}

            await next(context);
        }
    }
}
