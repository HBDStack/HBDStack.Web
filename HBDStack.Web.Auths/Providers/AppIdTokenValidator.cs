using System.Security.Claims;
using HBDStack.Web.Auths.JwtAuth;

namespace HBDStack.Web.Auths.Providers;

public class AppIdTokenValidator : ITokenValidator
{
    public ValueTask<StatusResult> ValidateAsync(string scheme, JwtAuthConfig config, ClaimsPrincipal principal)
    {
        if (!config.Authority.IsAzureAdAuthority()) return new ValueTask<StatusResult>(StatusResult.Success());

        var appId = principal.FindFirst("appid")?.Value;

        return new ValueTask<StatusResult>(appId != config.ClientId
            ? StatusResult.Fails($"The AppId of {scheme} is not matched.")
            : StatusResult.Success());
    }
}