using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Infrastucture.Authentication
{
    public class TestPolicy(IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<TestRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TestRequirement requirement)
        {
            var a = new List<AuthenticateResult>();
            //foreach (var r in requirement.Schemas)
            //    await httpContextAccessor.HttpContext.AuthenticateAsync();

            var r = await httpContextAccessor.HttpContext.AuthenticateAsync(Auth0Constants.AuthenticationScheme);

            if (r.Succeeded) 
            {
                httpContextAccessor.HttpContext.User = r.Principal;
                context.Succeed(requirement);
            }
            var r1 = await httpContextAccessor.HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
            if (r1.Succeeded)
            {
                httpContextAccessor.HttpContext.User = r1.Principal;
                context.Succeed(requirement);
            }
            context.Succeed(requirement);
        }
    }

    public class TestRequirement : IAuthorizationRequirement
    {
        public IEnumerable<string> Schemas { get; set; }

        public TestRequirement(IEnumerable<string> schemas)
        {
            Schemas = schemas;
        }
    }
}

