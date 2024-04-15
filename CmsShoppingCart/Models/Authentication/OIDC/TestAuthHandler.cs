using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System;

namespace CmsShoppingCart.WebApp.Models.Authentication.OIDC
{
    public class TestAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public string DisplayMessage { get; set; }
    }

    public class TestAuthenticationSchemeOptionsHandler : AuthenticationHandler<TestAuthenticationSchemeOptions>
    {
        public TestAuthenticationSchemeOptionsHandler(IOptionsMonitor<TestAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
