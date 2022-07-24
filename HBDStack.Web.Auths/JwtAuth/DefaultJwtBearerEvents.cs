using System.Security.Claims;
using HBDStack.Web.Auths.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;

namespace HBDStack.Web.Auths.JwtAuth;

public class DefaultJwtBearerEvents : JwtBearerEvents
{
    private readonly IReadOnlyCollection<IClaimsProvider> _claimsProviders;
    private readonly IReadOnlyCollection<ITokenValidator> _tokenValidators;
    private readonly ILogger<DefaultJwtBearerEvents> _logger;
    private readonly JwtAuthOptions _options;

    public DefaultJwtBearerEvents(IEnumerable<IClaimsProvider> claimsProviders,
        IEnumerable<ITokenValidator> tokenValidators,
        IOptions<JwtAuthOptions> options,
        ILogger<DefaultJwtBearerEvents> logger)
    {
        _options = options.Value;
        _tokenValidators = tokenValidators.ToList();
        _logger = logger;
        _claimsProviders = claimsProviders.ToList();
    }

    public override Task MessageReceived(MessageReceivedContext context)
    {
        var accessToken = context.Request.Query[ClaimsProviderKeys.AccessToken];

        // If the request is for our hub...
        var path = context.HttpContext.Request.Path;

        if (!string.IsNullOrEmpty(accessToken))
            context.Token = accessToken;

        return base.MessageReceived(context);
    }

    protected virtual async Task<bool> ValidateTokenAsync(TokenValidatedContext context, JwtAuthConfig config)
    {
        //Token validation
        foreach (var validator in _tokenValidators)
        {
            var rs = await validator.ValidateAsync(context.Scheme.Name, config, context.Principal!);
            if (rs.IsSuccess) continue;
            
            context.Fail(rs.Message!);
            return false;
        }

        return true;
    }

    protected virtual async Task<IList<Claim>> GetClaimsAsync(TokenValidatedContext context)
    {
        //Add claims to principal
        var claims = new List<Claim>();

        if (_claimsProviders.Any())
        {
            var results = await Task.WhenAll(_claimsProviders.Select(p => p.GetClaimsAsync(context.Scheme.Name,  context.Principal!)));
            
            claims.AddRange(results.SelectMany(c => c));
        }

        return claims;
    }
    
    public override async Task TokenValidated(TokenValidatedContext context)
    {
        var config = _options.TryGetConfig (context.Scheme.Name);
        //Execute custom validation
        if(! await ValidateTokenAsync(context,config))return;
        
        //Add custom claims to Identity
        var claims =await GetClaimsAsync(context);

        if (claims.Any())
            context.Principal!.AddIdentity(new ClaimsIdentity(claims.Distinct()));

        await base.TokenValidated(context);
    }

    public override Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        if (IdentityModelEventSource.ShowPII)
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            _logger.LogError(context.Exception, context.Exception.Message);

        return base.AuthenticationFailed(context);
    }
}