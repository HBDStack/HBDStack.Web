using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBD.Reactjs.Auths.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet(nameof(Login))]
    [AllowAnonymous]
    public IActionResult Login(string? scheme="Singa-AzureAD",string? returnUrl = "https://localhost:44452/api/info/tokens") 
        => Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, scheme!);
    
    [HttpGet(nameof(LogOut))]
    public async Task<IActionResult> LogOut(string? scheme="Singa-AzureAD",string? returnUrl = "https://localhost:44452")
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return SignOut(new AuthenticationProperties { RedirectUri = returnUrl }, scheme!);
    }
}