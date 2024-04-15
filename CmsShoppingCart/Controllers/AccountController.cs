using CmsShoppingCart.WebApp.Infrastucture;
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
using System;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace CmsShoppingCart.WebApp.Controllers
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
            this.signInManager = singInManager; ;
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
                    Email = user.Email
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
        public async Task<IActionResult> Login(string returnUrl)
        {
            var identityProviders = await db.SSOIdentityProviders.ToListAsync();
            var idpViewModels = identityProviders.Select(p => new IdentityProviderViewModel(p));

            var login = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                IdentityProviders = idpViewModels,
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

        //POST /account/LoginWithSSO
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task LoginWithSSO(string providerId)
        {
            if (string.IsNullOrEmpty(providerId))
            {
                ModelState.AddModelError("", "Cannot find inentity provider with id: " + providerId);
            }

            var result = await Request.HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

            if (!result.Succeeded)
            {
                await Request.HttpContext.ChallengeAsync(providerId, new AuthenticationProperties() { RedirectUri = "/account/ExternalHandler" });
            }
        }

        //POST /account/ExternalHandler
        [AllowAnonymous]
        public async Task<IActionResult> ExternalHandler()
        {
            var result = await Request.HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            var claimsDictionnary = result.Principal.Claims.ToDictionary(c => c.Type);

            var name = claimsDictionnary["name"];
            var email = claimsDictionnary["email"];

            var appUser = await userManeger.FindByEmailAsync(email.Value);

            if (appUser != null)
            {
                await signInManager.SignInWithClaimsAsync(appUser, false, result.Principal.Claims);
                return Redirect("/");
            }

            var newUser = new AppUser()
            {
                Ocupation = "Admin",
                Id = Guid.NewGuid().ToString(),
                UserName = name.Value,
                Email = email.Value,
            };

            var registerResult = await userManeger.CreateAsync(newUser);

            if (registerResult.Succeeded)
            {
                await signInManager.SignInWithClaimsAsync(newUser, false, result.Principal.Claims);
            }

            ModelState.AddModelError("", "Login with SSO failed!");
            return Redirect("/");
        }

        //GET /account/login
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Redirect("/");
        }

        //GET /account/edit
        public async Task<IActionResult> Edit()
        {
            AppUser appUser = await userManeger.FindByNameAsync(User.Identity.Name);
            UserEdit user = new UserEdit(appUser);

            return View(user);
        }

        //POST /account/edit
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEdit user)
        {
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
