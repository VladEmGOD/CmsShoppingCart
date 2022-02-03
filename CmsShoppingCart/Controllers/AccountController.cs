using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManeger;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(SignInManager<AppUser> singInManager, UserManager<AppUser> userManeger)
        {
            this.userManeger = userManeger;
            this._signInManager = singInManager;
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
        public IActionResult Login(string returnUrl) 
        {
            Login login = new Login
            {
                ReturnUrl = returnUrl
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
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager
                        .PasswordSignInAsync(appUser, login.Password, false, false);
                    if (result.Succeeded) return Redirect(login.ReturnUrl ?? "/");
                }

                ModelState.AddModelError("", "Login failed, wrong credentials");
            }

            return View(login);
        }

        //GET /account/login
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }
    }
}
