using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace HBDStack.AspNetCore.Extensions.JwtAuth;

/// <summary>
///
/// </summary>
public static class JwtAuthConfig
{
    #region Methods

    public static AuthenticationBuilder AddBearerAuth(this AuthenticationBuilder services, JwtAppAuth azApp)
        => services.AddBearerAuth<DefaultJwtBearerEvents>(azApp);

    public static AuthenticationBuilder AddBearerAuth(this AuthenticationBuilder services, string scheme, JwtAppAuth azApp)
        => services.AddBearerAuth<DefaultJwtBearerEvents>(scheme, azApp);

    public static AuthenticationBuilder AddBearerAuth<TEvents>(this AuthenticationBuilder services, JwtAppAuth azApp) where TEvents : DefaultJwtBearerEvents
        => services.AddBearerAuth<TEvents>(JwtBearerDefaults.AuthenticationScheme, azApp);

    /// <summary>
    /// Add with custom TEvents
    /// </summary>
    /// <typeparam name="TEvents"></typeparam>
    /// <param name="services"></param>
    /// <param name="scheme"></param>
    /// <param name="azApp"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddBearerAuth<TEvents>(this AuthenticationBuilder services, string scheme, JwtAppAuth azApp) where TEvents : DefaultJwtBearerEvents
    {
        services.Services.AddSingleton<TEvents>();
        services.AddJwtBearer(scheme, options =>
        {
            options.SaveToken = azApp.OverrideOptions?.SaveToken ?? false;
            options.RequireHttpsMetadata = azApp.OverrideOptions?.RequireHttpsMetadata ?? true;
            options.Authority = azApp.Authority;
            options.Audience = azApp.ClientId;
                 
            options.SaveToken = true;
            options.EventsType = typeof(TEvents);

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                // DO NOT usd this due to high risk Vulnerability Factor
                // Please Refer this: https://app.clickup.com/t/cxj60w
                // SignatureValidator = (t, p) => new JwtSecurityToken(t),

                RequireAudience = azApp.OverrideOptions?.RequireAudience ?? true,
                RequireExpirationTime = azApp.OverrideOptions?.RequireExpirationTime ?? true,
                RequireSignedTokens = azApp.OverrideOptions?.RequireSignedTokens ?? true,

                ValidateIssuer = azApp.OverrideOptions?.ValidateIssuer ?? azApp.Issuers?.Count > 0,
                ValidIssuers = azApp.Issuers,

                ValidAudiences = azApp.Audiences,
                ValidateAudience = azApp.OverrideOptions?.ValidateAudience ?? true,
                ValidateLifetime = azApp.OverrideOptions?.ValidateLifetime ?? true
            };
        });

        return services;
    }

    #endregion Methods
}