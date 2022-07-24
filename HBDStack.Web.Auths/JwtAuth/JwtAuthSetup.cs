using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using HBDStack.Web.Auths;
using HBDStack.Web.Auths.JwtAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///
/// </summary>
public static class JwtAuthSetup
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authConfig"></param>
    /// <param name="scheme"><see cref="JwtBearerDefaults.AuthenticationScheme"/></param>
    /// <param name="extraConfig"></param>
    /// <typeparam name="TEvents"></typeparam>
    /// <returns></returns>
    public static AuthenticationBuilder AddJwtAuth<TEvents>(this AuthenticationBuilder builder, JwtAuthConfig authConfig, string scheme = JwtBearerDefaults.AuthenticationScheme,Action<JwtBearerOptions>? extraConfig=null) where TEvents : DefaultJwtBearerEvents
    {
        builder.Services.TryAddScoped<TEvents>();

        void Options(JwtBearerOptions options)
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = true;
            options.Authority = authConfig.Authority;
            options.MetadataAddress = authConfig.MetaDataUrl ?? $"{authConfig.Authority}/.well-known/openid-configuration";
            options.Audience = authConfig.ClientId;
            options.RefreshOnIssuerKeyNotFound = true;

            extraConfig?.Invoke(options);
            
            options.EventsType = typeof(TEvents);

            if (!authConfig.Audiences.Any()) authConfig.Audiences.Add(authConfig.ClientId);
            if (!authConfig.Issuers.Any()) authConfig.Issuers.Add(authConfig.Authority);

            options.TokenValidationParameters.ValidIssuers = authConfig.Issuers;
            options.TokenValidationParameters.ValidIssuer = authConfig.Issuers.First();
            options.TokenValidationParameters.ValidAudiences = authConfig.Audiences;
            options.TokenValidationParameters.ValidateIssuerSigningKey = authConfig.ValidateIssuerSigningKey;

            if (!string.IsNullOrEmpty(authConfig.ClientSecret))
            {
                options.TokenValidationParameters.IssuerSigningKey =
                    new SymmetricSecurityKey(new HMACSHA512(Encoding.UTF8.GetBytes(authConfig.ClientSecret)).Key);
            }

            if (authConfig.Authority.IsAzureAdAuthority())
            {
                options.TokenValidationParameters.ValidateIssuerSigningKey = false;
                options.TokenValidationParameters.SignatureValidator = (t, p) => new JwtSecurityToken(t);
            }
        }

        return builder.AddJwtBearer(scheme, Options);
    }
    public static AuthenticationBuilder AddJwtAuth(this AuthenticationBuilder services, JwtAuthConfig authConfig, string scheme = JwtBearerDefaults.AuthenticationScheme,Action<JwtBearerOptions>? extraConfig=null)
       => services.AddJwtAuth<DefaultJwtBearerEvents>(authConfig, scheme,extraConfig);

    public static AuthenticationBuilder AddJwtAuths<TEvents>(this AuthenticationBuilder builder, JwtAuthOptions configuration, Action<string,JwtBearerOptions>? extraConfig = null) where TEvents : DefaultJwtBearerEvents
    {
        builder.Services.AddSingleton(Options.Options.Create(configuration));
        
        foreach (var config in configuration)
            builder.AddJwtAuth<TEvents>(config.Value, config.Key, o => extraConfig?.Invoke(config.Key, o));
        
        return builder;
    }

    public static AuthenticationBuilder AddJwtAuths(this AuthenticationBuilder builder, JwtAuthOptions configuration, Action<string, JwtBearerOptions>? extraConfig = null)
        => builder.AddJwtAuths<DefaultJwtBearerEvents>(configuration, extraConfig);
    
    public static AuthenticationBuilder AddJwtAuths<TEvents>(this AuthenticationBuilder builder, IConfiguration configuration, Action<string,JwtBearerOptions>? extraConfig = null) where TEvents : DefaultJwtBearerEvents
    {
        var config = configuration.Bind<JwtAuthOptions>(JwtAuthOptions.Name);
        return builder.AddJwtAuths<TEvents>(config,extraConfig);
    }
    
    public static AuthenticationBuilder AddJwtAuths(this AuthenticationBuilder builder, IConfiguration configuration, Action<string,JwtBearerOptions>? extraConfig = null) 
        => builder.AddJwtAuths<DefaultJwtBearerEvents>(configuration,extraConfig);
}