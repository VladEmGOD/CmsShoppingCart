using System.Collections.Generic;
using System.Security.Claims;

namespace CmsShoppingCart.WebApp.Models
{
    public static class ShopClaimTypes
    {
        public static string AuthenticationScheme = "AuthenticationScheme";

        public static List<string> SupportedDotNetClaims =
            [
                ClaimTypes.Email,
                ClaimTypes.Name,
                ClaimTypes.NameIdentifier,
                ClaimTypes.Sid,
            ];
    }

    public static class ShopClaimTypeReadbleCaptions
    {
        private static Dictionary<string, string> captions = new()
        {
            { ClaimTypes.Email,"Email" },
            { ClaimTypes.Name,"Name" },
            { ClaimTypes.NameIdentifier,"Name identifier" },
            { ClaimTypes.Sid, "Sid" },
        };

        public static string GetTranslation(string key) => captions[key]; 
    }
}
