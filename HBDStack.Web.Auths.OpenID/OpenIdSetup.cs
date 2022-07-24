using HBDStack.Web.Auths.OpenID;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class OpenIdSetup
{
    public static AuthenticationBuilder AddOpenIdAuth<TEvents>(this AuthenticationBuilder builder, OpenIdConfig authConfig, string scheme = OpenIdConnectDefaults.AuthenticationScheme, Action<OpenIdConnectOptions>? extraConfig=null)where TEvents:DefaultOpenIdEvents
    {
        builder.Services.TryAddScoped<TEvents>();
        
        builder.AddOpenIdConnect(scheme,scheme, options =>
        {
            options.ClientId = authConfig.ClientId;
            options.ClientSecret = authConfig.ClientSecret;
            options.Authority = authConfig.Authority;

            options.SkipUnrecognizedRequests = true;
            options.RequireHttpsMetadata = false;
            options.SaveTokens = true;
            options.UseTokenLifetime = true;
            options.GetClaimsFromUserInfoEndpoint = authConfig.GetClaimsFromUserInfoEndpoint;
                            
            options.MetadataAddress =
                authConfig.MetaDataUrl ?? $"{authConfig.Authority}/.well-known/openid-configuration";
            
            options.ResponseMode = authConfig.ResponseMode;
            options.ResponseType = authConfig.ResponseType;

            if (authConfig.Scopes?.Any() == true)
            {
                options.Scope.Clear();
                foreach (var scope in authConfig.Scopes)
                    options.Scope.Add(scope);
            }

            options.CallbackPath = "/auth/signin-oidc";
            extraConfig?.Invoke(options);
            options.EventsType = typeof(TEvents);
        });

        return builder;
    }

    public static AuthenticationBuilder AddOpenIdAuth(this AuthenticationBuilder builder, OpenIdConfig authConfig, string scheme = OpenIdConnectDefaults.AuthenticationScheme, Action<OpenIdConnectOptions>? extraConfig = null)
        => builder.AddOpenIdAuth<DefaultOpenIdEvents>(authConfig, scheme, extraConfig);
    
    public static AuthenticationBuilder AddOpenIdAuths<TEvents>(this AuthenticationBuilder builder, OpenIdOptions configuration, Action<string,OpenIdConnectOptions>? extraConfig = null) where TEvents : DefaultOpenIdEvents
    {
        builder.Services.AddSingleton(Options.Options.Create(configuration));
        foreach (var config in configuration)
            builder.AddOpenIdAuth<TEvents>(config.Value, config.Key, o => extraConfig?.Invoke(config.Key, o));
        return builder;
    }

    public static AuthenticationBuilder AddOpenIdAuths(this AuthenticationBuilder builder, OpenIdOptions configuration, Action<string, OpenIdConnectOptions>? extraConfig = null)
        => builder.AddOpenIdAuths<DefaultOpenIdEvents>(configuration, extraConfig);
    
    public static AuthenticationBuilder AddOpenIdAuths<TEvents>(this AuthenticationBuilder builder, IConfiguration configuration, Action<string, OpenIdConnectOptions>? extraConfig = null) where TEvents : DefaultOpenIdEvents
    {
        var config = configuration.Bind<OpenIdOptions>(OpenIdOptions.Name);
        return builder.AddOpenIdAuths<TEvents>(config, extraConfig);
    }

    public static AuthenticationBuilder AddOpenIdAuths(this AuthenticationBuilder builder, IConfiguration configuration, Action<string, OpenIdConnectOptions>? extraConfig = null)
        => builder.AddOpenIdAuths<DefaultOpenIdEvents>(configuration, extraConfig);
}