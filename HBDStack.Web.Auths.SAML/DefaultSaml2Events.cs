using System.Security.Claims;
using HBDStack.Web.Auths.Providers;
using Sustainsys.Saml2.Saml2P;
using Sustainsys.Saml2.WebSso;

namespace HBDStack.Web.Auths.SAML;

public class DefaultSaml2Events
{
    private readonly IEnumerable<IClaimsProvider> _claimsProviders;

    public DefaultSaml2Events(IEnumerable<IClaimsProvider> claimsProviders) => _claimsProviders = claimsProviders;

    public virtual void Authenticated(CommandResult context, Saml2Response response)
    {
        if (context.Principal == null) return;
          
        //Add custom claims to principal
        var claims = new List<Claim>();

        if (_claimsProviders.Any())
        {
            var results = Task.WhenAll(_claimsProviders.Select(p => p.GetClaimsAsync(response.Id.Value, context.Principal!))).GetAwaiter().GetResult();
            claims.AddRange(results.SelectMany(c => c));
        }

        if (claims.Any())
            context.Principal!.AddIdentity(new ClaimsIdentity(claims.Distinct()));
    }
}