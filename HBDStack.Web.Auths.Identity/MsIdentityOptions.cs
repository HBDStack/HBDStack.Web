using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HBDStack.Web.Auths.Identity;

public class MsIdentityOptions
{
    public Action<IdentityOptions>? Options { get; set; }
    public Action<CookieAuthenticationOptions>? CookieOptions { get; set; }

    public Action<DbContextOptionsBuilder>? DbContextOptions { get; set; }
}