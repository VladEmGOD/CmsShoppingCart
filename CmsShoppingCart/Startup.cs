using Auth0.AspNetCore.Authentication;
using CmsShoppingCart.WebApp.Infrastucture;
using CmsShoppingCart.WebApp.Infrastucture.Middlewares;
using CmsShoppingCart.WebApp.Infrastucture.Providers;
using CmsShoppingCart.WebApp.Infrastucture.Services;
using CmsShoppingCart.WebApp.Models;
using CmsShoppingCart.WebApp.Models.Authentication.OIDC;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;
using System.Threading.Tasks;


namespace CmsShoppingCart.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSession(options =>
            {
                //options.IdleTimeout = TimeSpan.FromSeconds(2);
                //options.IdleTimeout = TimeSpan.FromDays(2);
            });

            services.AddLogging();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllersWithViews();

            services.AddScoped<IMediaImageProvider, MediaImageProvider>();

            services.AddDbContext<CmsShoppingCartContext>
                (options => options.UseSqlServer
            (Configuration.GetConnectionString("CmsShoppingCartContext")));

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;

            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<CmsShoppingCartContext>()
                .AddDefaultTokenProviders();

            var b = new AuthenticationBuilder(services);

            b.AddOpenIdConnect(SuportedAuthSchemas.Auth0, o =>
            {
                o.Authority = Configuration["SSO:Auth0:Authority"];
                o.ClientId = Configuration["SSO:Auth0:ClientId"];
                o.ClientSecret = Configuration["SSO:Auth0:ClientSecret"];
                o.CallbackPath = Configuration["SSO:Auth0:CallbackPath"];
                o.Scope.Add("email");
                o.UsePkce = true;
                o.ResponseType = OpenIdConnectResponseType.Code;
                o.SignInScheme = IdentityConstants.ApplicationScheme;

                o.ClaimActions.Add(new SimpleClaimAction("AuthenticationType", SuportedAuthSchemas.Auth0));
                o.TokenValidationParameters.RoleClaimType = "account_role";

                o.Events = new OpenIdConnectEvents()
                {
                    OnRedirectToIdentityProviderForSignOut = context =>
                    {
                        var logoutUri = "/";
                        context.Response.Redirect(logoutUri);
                        context.HandleResponse();

                        return Task.CompletedTask;
                    }
                };

            });

            b.AddOpenIdConnect(SuportedAuthSchemas.Okta, o =>
            {
                o.Authority = Configuration["SSO:Okta:Authority"];
                o.ClientId = Configuration["SSO:Okta:ClientId"];
                o.ClientSecret = Configuration["SSO:Okta:ClientSecret"];
                o.CallbackPath = Configuration["SSO:Okta:CallbackPath"];
                o.Scope.Add("email");
                o.UsePkce = true;
                o.ResponseType = OpenIdConnectResponseType.Code;
                o.SignInScheme = IdentityConstants.ApplicationScheme;

                o.ClaimActions.Add(new SimpleClaimAction("AuthenticationType", SuportedAuthSchemas.Okta));
                o.TokenValidationParameters.RoleClaimType = "account_role";

                o.Events = new OpenIdConnectEvents()
                {
                    OnRedirectToIdentityProviderForSignOut = context =>
                    {
                        var logoutUri = "/";
                        context.Response.Redirect(logoutUri);
                        context.HandleResponse();

                        return Task.CompletedTask;
                    }
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            //app.UseMiddleware<OIDCIdentityProvidersWiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("productsC", "products/",
                    defaults: new { controller = "Products", action = "index" });

                endpoints.MapControllerRoute("Account", "account/",
                   defaults: new { controller = "Account", action = "index" });

                endpoints.MapControllerRoute("Orders", "orders/",
                   defaults: new { controller = "Orders", action = "index" });

                endpoints.MapControllerRoute("cart", "cart/",
                    defaults: new { controller = "Cart", action = "index" });

                endpoints.MapControllerRoute("pages", "{slug?}", defaults: new { controller = "Pages", action = "Page" });

                endpoints.MapControllerRoute("products", "products/{slug}",
                    defaults: new { controller = "Products", action = "ProductsByCategory" });

                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                 name: "default",
                 pattern: "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}
