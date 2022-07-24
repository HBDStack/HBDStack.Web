using System.Security.Claims;
using HBDStack.Web.Auths.Providers;

namespace Web.Tests.Handlers;

public class ClaimsProvider : IClaimsProvider
{
    public Task<IEnumerable<Claim>> GetClaimsAsync(string scheme, ClaimsPrincipal principal)
    {
        var list = new List<Claim>();
        
        if (scheme == "Singa-AzureAD")
            list.Add(new Claim(ClaimTypes.Role, "Admin"));
        else list.Add(new Claim(ClaimTypes.Role, "ReadOnly"));
        
        return Task.FromResult<IEnumerable<Claim>>(list);
    }
}