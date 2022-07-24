using System.Security.Claims;
using HBDStack.Web.Auths.Providers;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace HBDStack.Web.Auths.CookieAuth;

public class DefaultCookieEvents : CookieAuthenticationEvents
{
     private readonly IEnumerable<IClaimsProvider> _claimsProviders;

     public DefaultCookieEvents(IEnumerable<IClaimsProvider> claimsProviders) => _claimsProviders = claimsProviders;
     
     public override async Task SigningIn(CookieSigningInContext context)
     {
          await base.SigningIn(context);
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