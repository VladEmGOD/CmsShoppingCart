using CmsShoppingCart.WebApp.Models.Authentication.OIDC;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using CmsShoppingCart.WebApp.Infrastucture.Providers;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection.Metadata;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace CmsShoppingCart.WebApp.Infrastucture.Services
{
    public interface IIdentityProvidersManager
    {
        public Task AddProvider(IdentityProvider provider);
    }

    public sealed class IdentityProvidersManager(
        IAuthenticationSchemeProvider schemeProvider,
        IOptionsMonitorCache<OpenIdConnectOptions> optionsCache,
        IOptionsMonitor<OpenIdConnectOptions> options,
        OpenIdConnectPostConfigureOptions postConfigureOptions
        ) : IIdentityProvidersManager
    {
        public async Task AddProvider(IdentityProvider provider)
        {
            var newProviderId = provider.Id.ToString();

            //var t1 = options.Get(newProviderId);

            var o = new OpenIdConnectOptions();
            o.Authority = provider.Url;
            o.ClientId = provider.ApplicationId;
            o.ClientSecret = provider.ClientSecret;
            o.MapInboundClaims = false;
            o.SignInScheme = IdentityConstants.ExternalScheme;
            
            o.Scope.Add("email");

            optionsCache.TryAdd(newProviderId, o);
            postConfigureOptions.PostConfigure(newProviderId, o);

            var scheme = await schemeProvider.GetSchemeAsync(newProviderId);

            if (scheme == null)
            {
                schemeProvider.AddScheme(new AuthenticationScheme(newProviderId, provider.Name, typeof(OpenIdConnectHandler)));
                var a = optionsCache.GetOrAdd(newProviderId, () => o);
            }

            var t2 = options.Get(newProviderId);
        }

        public class TestHandler : OpenIdConnectHandler
        {
            readonly IOptionsMonitorCache<OpenIdConnectOptions> optionsCache;
            readonly IHttpContextAccessor httpContextAccessor;

            public TestHandler(
                IHttpContextAccessor httpContextAccessor,
                IOptionsMonitorCache<OpenIdConnectOptions> optionsCache,
                IOptionsMonitor<OpenIdConnectOptions> options,
                ILoggerFactory logger,
                HtmlEncoder htmlEncoder,
                UrlEncoder encoder)
                : base(options, logger, htmlEncoder, encoder)
            {
                this.optionsCache = optionsCache;
                this.httpContextAccessor = httpContextAccessor;
            }

            public TestHandler(
                IHttpContextAccessor httpContextAccessor,
                IOptionsMonitorCache<OpenIdConnectOptions> optionsCache,
                IOptionsMonitor<OpenIdConnectOptions> options,
                ILoggerFactory logger,
                HtmlEncoder htmlEncoder,
                UrlEncoder encoder,
                ISystemClock clock)
                : base(options, logger, htmlEncoder, encoder, clock)
            {
                this.optionsCache = optionsCache;
                this.httpContextAccessor = httpContextAccessor;
            }

            public override Task<bool> HandleRequestAsync()
            {
                var httpContext = httpContextAccessor.HttpContext.Request.Query;
                var a = httpContext["providerId"];
                ;
                return base.HandleRequestAsync();
            }
        }
    }
}
