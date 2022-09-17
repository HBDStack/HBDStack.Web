using HBDStack.Web.Auths.JwtAuth;
using HBDStack.Web.Auths.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Web.Auth.Configs;

public class JwtAuthHandler : DefaultJwtBearerEvents
{
    public JwtAuthHandler(IEnumerable<IClaimsProvider> claimsProviders, IEnumerable<ITokenValidator> tokenValidators, IOptions<JwtAuthOptions> options, ILogger<DefaultJwtBearerEvents> logger) : base(claimsProviders, tokenValidators,
        options, logger)
    {
    }
    
    public override async Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        await base.AuthenticationFailed(context);
        await Console.Error.WriteLineAsync(context.Exception.Message);
    }
}