using System.Security.Claims;
using HBDStack.Web.Auths.Providers;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace HBD.Reactjs.Auths.Providers;

public class ClaimsProvider : IClaimsProvider
{
    public Task<IEnumerable<Claim>> GetClaimsAsync(string scheme, ClaimsPrincipal principal)
    {
        if (string.Equals(scheme, CookieAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase)) return Task.FromResult(Enumerable.Empty<Claim>());
        return Task.FromResult<IEnumerable<Claim>>(new[] { new Claim(ClaimTypes.Role, $"{scheme}_Admin") });
    }
}