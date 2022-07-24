using System.Security.Claims;
using HBDStack.Web.Auths.Providers;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace HBDStack.Web.Auths.OpenID;

public class DefaultOpenIdEvents : OpenIdConnectEvents
{
    private readonly IEnumerable<IClaimsProvider> _claimsProviders;

    public DefaultOpenIdEvents(IEnumerable<IClaimsProvider> claimsProviders) => _claimsProviders = claimsProviders;

    public override async Task TokenValidated(TokenValidatedContext context)
    {
        await base.TokenValidated(context);
        if (context.Principal == null) return;
          
        //Add custom claims to principal
        var claims = new List<Claim>();

        if (_claimsProviders.Any())
        {
            var results =
                await Task.WhenAll(_claimsProviders.Select(p => p.GetClaimsAsync(context.Scheme.Name, context.Principal!)));
            claims.AddRange(results.SelectMany(c => c));
        }

        if (claims.Any())
            context.Principal!.AddIdentity(new ClaimsIdentity(claims.Distinct()));
    }
}