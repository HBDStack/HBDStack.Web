namespace HBDStack.AspNetCore.Extensions.JwtAuth;

public class AuthOptions
{
    #region Properties

    public bool IncludeErrorDetails { get; set; }

    public bool RequireAudience { get; set; } = true;

    public bool RequireExpirationTime { get; set; } = true;

    public bool RequireHttpsMetadata { get; set; } = true;

    public bool RequireSignedTokens { get; set; } = true;

    public bool SaveToken { get; set; }

    public bool ValidateAudience { get; set; } = true;

    public bool ValidateIssuer { get; set; } = true;

    public bool ValidateLifetime { get; set; } = true;

    #endregion Properties
}