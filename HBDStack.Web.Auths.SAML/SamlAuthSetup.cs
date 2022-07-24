using HBDStack.Web.Auths.SAML;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.Metadata;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class SamlAuthSetup
{
    public static AuthenticationBuilder AddSaml2Auth<TEvents>(this AuthenticationBuilder builder, SamlAuthConfig authConfig, string scheme = Saml2Defaults.Scheme, Action<Saml2Options>? extraConfig = null) where TEvents : DefaultSaml2Events
    {
        builder.Services.TryAddScoped<TEvents>();

        builder.AddSaml2(scheme, options =>
        {
            options.SPOptions.ModulePath = "/auth/Saml2";
            extraConfig?.Invoke(options);

            options.Notifications.AcsCommandResultCreated = (context, response) =>
            {
                using var provider = builder.Services.BuildServiceProvider();
                var events = provider.GetRequiredService<TEvents>();
                events.Authenticated(context, response);
            };
            
            options.SPOptions.EntityId = new EntityId(authConfig.EntityId);
            options.IdentityProviders.Add(
                new IdentityProvider(
                    new EntityId(authConfig.IdentityUrl), options.SPOptions)
                {
                    MetadataLocation = authConfig.MetaDataUrl
                });
        });

        return builder;
    }

    public static AuthenticationBuilder AddSaml2Auth(this AuthenticationBuilder builder, SamlAuthConfig authConfig, string scheme = Saml2Defaults.Scheme, Action<Saml2Options>? extraConfig = null)
        => builder.AddSaml2Auth<DefaultSaml2Events>(authConfig, scheme, extraConfig);

    public static AuthenticationBuilder AddSaml2Auths<TEvents>(this AuthenticationBuilder builder, SamlAuthOptions configuration, Action<string, Saml2Options>? extraConfig = null) where TEvents : DefaultSaml2Events
    {
        builder.Services.AddSingleton(Options.Options.Create(configuration));
        foreach (var config in configuration)
            builder.AddSaml2Auth<TEvents>(config.Value, config.Key, o => extraConfig?.Invoke(config.Key, o));
        return builder;
    }

    public static AuthenticationBuilder AddSaml2Auths(this AuthenticationBuilder builder, SamlAuthOptions configuration, Action<string, Saml2Options>? extraConfig = null)
        => builder.AddSaml2Auths<DefaultSaml2Events>(configuration, extraConfig);

    public static AuthenticationBuilder AddSaml2Auths<TEvents>(this AuthenticationBuilder builder, IConfiguration configuration, Action<string, Saml2Options>? extraConfig = null) where TEvents : DefaultSaml2Events
    {
        var config = configuration.Bind<SamlAuthOptions>(SamlAuthOptions.Name);
        return builder.AddSaml2Auths<TEvents>(config, extraConfig);
    }

    public static AuthenticationBuilder AddSaml2Auths(this AuthenticationBuilder builder, IConfiguration configuration, Action<string, Saml2Options>? extraConfig = null)
        => builder.AddSaml2Auths<DefaultSaml2Events>(configuration, extraConfig);
}