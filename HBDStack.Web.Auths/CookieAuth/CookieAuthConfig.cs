using Microsoft.AspNetCore.Authentication.Cookies;

namespace HBDStack.Web.Auths.CookieAuth;

public class CookieAuthConfig
{
    public bool IncludesLaxPolicy { get; set; } = true;
    public  Action<CookieAuthenticationOptions>? Options { get; set; }= op =>
    {
        op.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        op.SlidingExpiration = true;
    };
}