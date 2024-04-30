using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using System.Security.Claims;
using System.Text.Json;

namespace CmsShoppingCart.WebApp.Models.Authentication.OIDC
{
    public class SimpleClaimAction : ClaimAction
    {
        private string SavedValue { get; set; }

        public SimpleClaimAction(string claimType, string valueType) : base(claimType, valueType)
        {
            SavedValue = valueType;
        }

        public override void Run(JsonElement userData, ClaimsIdentity identity, string issuer)
        {
            identity.AddClaim(new Claim(ShopClaimTypes.AuthenticationScheme, SavedValue));
        }
    }
}
