using Auth0.AspNetCore.Authentication;
using CmsShoppingCart.WebApp.Infrastucture;
using CmsShoppingCart.WebApp.Infrastucture.Authentication;
using CmsShoppingCart.WebApp.Models;
using CmsShoppingCart.WebApp.Models.Authentication;
using CmsShoppingCart.WebApp.Models.Authentication.OIDC;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Controllers.Authentication
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManeger;
        private readonly SignInManager<AppUser> signInManager;
        private IPasswordHasher<AppUser> passwordHasher;
        private CmsShoppingCartContext db;
        private readonly IAuthenticationSchemeProvider schemeProvider;

        public AccountController(SignInManager<AppUser> singInManager,
                                UserManager<AppUser> userManeger,
                                IPasswordHasher<AppUser> passwordHasher,
                                CmsShoppingCartContext db, IAuthenticationSchemeProvider schemeProvider)
        {
            this.userManeger = userManeger;
            signInManager = singInManager; ;
            this.passwordHasher = passwordHasher;
            this.db = db;
            this.schemeProvider = schemeProvider;
        }

        //GET /account/register
        [AllowAnonymous]
        public IActionResult Register() => View();

        //POST /account/register
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    AuthenticationType = AuthenticationType.Internal,
                };

                IdentityResult result = await userManeger.CreateAsync(appUser, user.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

            }

            return View(user);
        }

        //GET /account/login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            var login = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                IdentityProviders = [],
            };

            return View(login);
        }

        //POST /account/login
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = await userManeger.FindByEmailAsync(login.Email);
                if (appUser != null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager
                        .PasswordSignInAsync(appUser, login.Password, false, false);

                    if (result.Succeeded)
                        return Redirect(login.ReturnUrl ?? "/");
                }

                ModelState.AddModelError("", "Login failed, wrong credentials");
            }

            return View(login);
        }

        //POST /account/login
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task LoginWithSSO(string authenticationScheme, string returnUrl = "/")
        {
            if (authenticationScheme.IsNullOrEmpty())
                throw new ArgumentException("Scheme cannot be null or empty", nameof(authenticationScheme));

            var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
              .WithRedirectUri(returnUrl)
              .Build();

            await HttpContext.ChallengeAsync(
              authenticationScheme,
              authenticationProperties
            );
        }

        //GET /account/Logout
        public async Task<IActionResult> Logout()
        {

            var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
              .WithRedirectUri(Url.Action("Index", "Home"))
              .Build();

            var authClaim = User.Claims.FirstOrDefault(c => c.Type == ShopClaimTypes.AuthenticationScheme);
            if (authClaim is not null)
                await HttpContext.SignOutAsync(authClaim.Value, authenticationProperties);

            await signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        //GET /account/edit
        // [Authorize(Policy = "TestPolicy")]
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var viewModel = new UserEditViewModel();
            var authClaim = User.Claims.FirstOrDefault(c => c.Type == ShopClaimTypes.AuthenticationScheme);

            if (authClaim is null)
            {
                var appUser = await userManeger.FindByNameAsync(User.Identity.Name);
                viewModel.EditUserInfo = new UserEdit(appUser);
                viewModel.IsOIDCAuthentication = false;
            }
            else
            {
                viewModel.IsOIDCAuthentication = true;
                viewModel.OIDCClaims.Add("Authentication type", authClaim.Value.ToString());

                foreach (var claim in User.Claims)
                {
                    if (ShopClaimTypes.SupportedDotNetClaims.Contains(claim.Type))
                    {
                        var caption = ShopClaimTypeReadbleCaptions.GetTranslation(claim.Type);
                        viewModel.OIDCClaims.Add(caption, claim.Value.ToString());
                    }
                    else if (claim.Type == "picture")
                        viewModel.PictureUrl = claim.Value.ToString();
                    else
                        viewModel.OIDCClaims.Add(claim.Type, claim.Value.ToString());
                }
            }

            return View(viewModel);
        }

        //POST /account/edit
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEdit user)
        {
            var userT = User;
            AppUser appUser = await userManeger.FindByNameAsync(User.Identity.Name);

            if (ModelState.IsValid)
            {
                appUser.Email = user.Email;

                if (user.Password != null)
                {
                    appUser.PasswordHash = passwordHasher.HashPassword(appUser, user.Password);
                }

                IdentityResult result = await userManeger.UpdateAsync(appUser);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Your information has been edited!";
                }
            }

            return View();
        }
    }
}
