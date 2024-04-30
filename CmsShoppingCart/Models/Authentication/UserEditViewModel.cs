using System.Collections.Generic;

namespace CmsShoppingCart.WebApp.Models.Authentication
{
    public class UserEditViewModel
    {
        public string PictureUrl { get; set; }

        public bool IsOIDCAuthentication { get; set; }

        public Dictionary<string, string> OIDCClaims { get; set; } = new Dictionary<string, string>();

        public UserEdit EditUserInfo { get; set; }
    }
}
