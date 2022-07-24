using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// ReSharper disable ClassNeverInstantiated.Global

namespace HBDStack.AspNetCore.Extensions.JwtAuth;

public class JwtTokenClaimsProvider : IJwtClaimsProvider
{
    #region Methods

    /// <summary>
    /// Get authentication and graph_token from Headers. <see cref="ClaimsProviderKeys"/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static Tokens GetTokens(TokenValidatedContext context)
    {
        if (context == null)
        {
            Trace.TraceError($"JwtTokenClaims: The {nameof(TokenValidatedContext)} is null.");
            return null;
        }

        var tokens = new Tokens
        {
            AccessToken = (context.SecurityToken as JwtSecurityToken)?.RawData
        };

        if (context.HttpContext.Request.Headers.ContainsKey(ClaimsProviderKeys.GraphToken))
        {
            tokens.GraphToken = context.HttpContext.Request.Headers[ClaimsProviderKeys.GraphToken];
        }
        else Trace.TraceInformation($"JwtTokenClaims: The {ClaimsProviderKeys.GraphToken} is not found, will try with Beare token.");

        return tokens;
    }

    public Task<ICollection<Claim>> GetClaimsAsync(ResultContext<JwtBearerOptions> context)
    {
        var token = GetTokens(context as TokenValidatedContext);
        var list = new List<Claim>();

        //Add token to claims allows to access from API.
        if (!string.IsNullOrEmpty(token.GraphToken))
        {
            Trace.TraceInformation($"JwtTokenClaims: The {ClaimsProviderKeys.GraphToken} added to Claims.");
            list.Add(new Claim(ClaimsProviderKeys.GraphToken, token.GraphToken));
        }

        if (!string.IsNullOrEmpty(token.AccessToken))
        {
            Trace.TraceInformation($"JwtTokenClaims: The {ClaimsProviderKeys.AccessToken} added to Claims.");
            list.Add(new Claim(ClaimsProviderKeys.AccessToken, token.AccessToken));
        }

        return Task.FromResult<ICollection<Claim>>(list);
    }

    #endregion Methods
}