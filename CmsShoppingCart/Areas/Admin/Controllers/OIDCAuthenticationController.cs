using CmsShoppingCart.Models.Authentication.OIDC;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace CmsShoppingCart.Areas.Admin.Controllers;

[Area("Admin")]
public class OIDCAuthenticationController : Controller
{
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IOptionsMonitorCache<TestAuthenticationSchemeOptions> _optionsCache;

    public OIDCAuthenticationController(IAuthenticationSchemeProvider schemeProvider, IOptionsMonitorCache<TestAuthenticationSchemeOptions> optionsCache)
    {
        _schemeProvider = schemeProvider;
        _optionsCache = optionsCache;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Remove(string scheme)
    {
        _schemeProvider.RemoveScheme(scheme);
        _optionsCache.TryRemove(scheme);
        return Redirect("/");
    }

    [HttpPost]
    public async Task<IActionResult> Add(string scheme, string optionsMessage)
    {
        if (await _schemeProvider.GetSchemeAsync(scheme) == null)
        {
            _schemeProvider.AddScheme(new AuthenticationScheme(scheme, scheme, typeof(TestAuthenticationSchemeOptions)));
        }
        else
        {
            _optionsCache.TryRemove(scheme);
        }
        _optionsCache.TryAdd(scheme, new TestAuthenticationSchemeOptions { DisplayMessage = optionsMessage });
        return Redirect("/");
    }

    [HttpPost]
    public async Task<IActionResult> Update(string scheme, string optionsMessage)
    {
        if (await _schemeProvider.GetSchemeAsync(scheme) == null)
        {
            _schemeProvider.AddScheme(new AuthenticationScheme(scheme, scheme, typeof(TestAuthenticationSchemeOptions)));
        }
        else
        {
            _optionsCache.TryRemove(scheme);
        }
        _optionsCache.TryAdd(scheme, new TestAuthenticationSchemeOptions { DisplayMessage = optionsMessage });
        return Redirect("/");
    }

}
