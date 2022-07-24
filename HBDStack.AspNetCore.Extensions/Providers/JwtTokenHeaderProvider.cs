using HBDStack.AzProxy.Core.Providers;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace HBDStack.AspNetCore.Extensions.Providers;

public class JwtTokenHeaderProvider : AuthTokenHeaderProvider
{
    #region Fields

    private readonly IHttpContextAccessor accessor;

    #endregion Fields

    #region Constructors

    public JwtTokenHeaderProvider(IHttpContextAccessor accessor) => this.accessor = accessor;

    #endregion Constructors

    #region Methods

    protected override ValueTask<string> GetTokenAsync()
    {
        var tokenKey = ClaimsProviderKeys.AccessToken;
        var token = accessor.HttpContext.User.Identity.IsAuthenticated ? accessor.HttpContext.User.FindFirst(tokenKey)?.Value : null;

        if (string.IsNullOrEmpty(token))
        {
            Trace.TraceInformation("JwtTokenHeaderProvider: The auth token is null or not authenticated.");
        }

        return new ValueTask<string>(token);
    }

    #endregion Methods
}