using System.Security.Claims;
using HBDStack.Web.Auths.JwtAuth;

namespace HBDStack.Web.Auths.Providers;

public class TokenClaimsValidator : ITokenValidator
{
    public ValueTask<StatusResult> ValidateAsync(string scheme, JwtAuthConfig config, ClaimsPrincipal principal)
    {
        if (config.ClaimsValidations == null) return new ValueTask<StatusResult>(StatusResult.Success());

        foreach (var validation in config.ClaimsValidations)
        {
            var value = principal.FindFirst(validation.Key)?.Value;
            if (value == null || !validation.Value.Contains(value))
                return new ValueTask<StatusResult>(
                    StatusResult.Fails($"The value of {validation.Key} is not found or invalid."));
        }

        return new ValueTask<StatusResult>(StatusResult.Success());
    }
}