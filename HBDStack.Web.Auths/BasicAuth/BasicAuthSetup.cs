using HBDStack.Web.Auths.BasicAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class BasicAuthSetup
{
    public static AuthenticationBuilder AddInternalBasicAuth<TEvents>(this AuthenticationBuilder builder, string schema = BasicAuthDefaults.Scheme)
        where TEvents : BasicAuthEvents
    {
        builder.Services.TryAddScoped<TEvents>();
        builder.Services.TryAddSingleton<IBasicAuthValidator, BasicAuthValidator>();
        builder.AddScheme<AuthenticationSchemeOptions, TEvents>(schema, null);

        return builder;
    }

    public static AuthenticationBuilder AddBasicAuth<TEvents>(this AuthenticationBuilder builder, IConfiguration configuration, string schema = BasicAuthDefaults.Scheme)
        where TEvents : BasicAuthEvents
    {
        builder.Services.Configure<BasicAuthOptions>(configuration.GetSection(BasicAuthOptions.Name));
        builder.AddInternalBasicAuth<TEvents>(schema);

        return builder;
    }

    public static AuthenticationBuilder AddBasicAuth(this AuthenticationBuilder builder, IConfiguration configuration, string schema = BasicAuthDefaults.Scheme)
        => builder.AddBasicAuth<BasicAuthEvents>(configuration, schema);


    public static AuthenticationBuilder AddBasicAuth<TEvents>(this AuthenticationBuilder builder, BasicAuthOptions configuration, string schema = BasicAuthDefaults.Scheme)
        where TEvents : BasicAuthEvents
    {
        builder.Services.AddSingleton(Options.Options.Create(configuration));
        builder.AddInternalBasicAuth<TEvents>(schema);

        return builder;
    }

    public static AuthenticationBuilder AddBasicAuth(this AuthenticationBuilder builder, BasicAuthOptions configuration, string schema = BasicAuthDefaults.Scheme)
        => builder.AddBasicAuth<BasicAuthEvents>(configuration, schema);
}