using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Infrastucture.Authentication;

public class AuthenticationStateProvider : ServerAuthenticationStateProvider
{
    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Call the base to get the AuthState and the user provided in the Security Headers by the server
        var authstate = await base.GetAuthenticationStateAsync();
        var user = authstate.User;

        if (user?.Identity?.IsAuthenticated ?? false)
        {
            // Do whatever you want here to retrieve the additional user information you want to
            // include in the ClaimsPrincipal - probably some form of Identity Service

            // Construct a ClaimsIdentity instance to attach to the ClaimsPrincipal
            // I just added a role as an example
            var myIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "User") });
            // Add it to the existing ClaimsPrincipal
            user.AddIdentity(myIdentity);
        }

        // construct a new state with the updated ClaimsPrincipal
        // - or an empty one of you didn't get a user in the first place
        // All the Authorization components and classes will now use this ClaimsPrincipal 
        return new AuthenticationState(user ?? new ClaimsPrincipal());
    }

}
