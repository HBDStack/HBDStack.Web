using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using HBDStack.Web.Auths.Providers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HBDStack.Web.Auths.BasicAuth;

public class BasicAuthEvents : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string AuthorizationHeaderKey = "Authorization";

    public BasicAuthEvents(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
     
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // skip authentication if endpoint has [AllowAnonymous] attribute
        var endpoint = Context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            return AuthenticateResult.NoResult();

        if (!Request.Headers.ContainsKey(AuthorizationHeaderKey))
            return AuthenticateResult.Fail("Missing Authorization Header");

        string? userName;
        string? password;
        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers[AuthorizationHeaderKey]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter!);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
            userName = credentials[0];
            password = credentials[1];
        }
        catch
        {
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }

        var validator = Context.RequestServices.GetRequiredService<IBasicAuthValidator>();
        var rs = validator.Validate(userName, password);
        if (!rs.success)
            return AuthenticateResult.Fail(rs.error!);
        
        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, userName),
            new(ClaimTypes.Name, userName),
        };
        
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        
        var claimResults =await Task.WhenAll( Context.RequestServices.GetServices<IClaimsProvider>()
            .Select( p=> p.GetClaimsAsync(Scheme.Name,principal)));
        
       claims.AddRange(claimResults.SelectMany(r=>r));
       return AuthenticateResult.Success(ticket);
    }
}