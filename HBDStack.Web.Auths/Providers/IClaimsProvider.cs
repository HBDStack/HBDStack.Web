using System.Security.Claims;

namespace HBDStack.Web.Auths.Providers;

public interface IClaimsProvider
{
    Task<IEnumerable<Claim>> GetClaimsAsync(string scheme,  ClaimsPrincipal principal);
}