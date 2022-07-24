using HBDStack.Web.Auths;
using HBDStack.Web.Auths.CertAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// If running in localhost. Ensure the client certificate is added to Trusted Root Certification of Computer Store.
/// http://brainof-dave.blogspot.com/2008/08/remote-certificate-is-invalid-according.html
/// </summary>
public static class CertAuthSetup
{
    private static AuthenticationBuilder AddInternalCertAuth<TEvents>(this AuthenticationBuilder builder, CertAuthConfig? config = null)
        where TEvents : CertificateAuthenticationEvents
    {
        config ??= new CertAuthConfig();

        builder.Services.TryAddScoped<TEvents>();
        builder.Services.AddCertificateForwarding(cf =>
            {
                cf.CertificateHeader = config.CertificateForwardingHeader;
                cf.HeaderConverter = AspCoreExtensions.ConvertBase64ToCert!;
            });

        builder
            .AddCertificate(c =>
            {
                config.Scheme = config.Scheme;
                config.ConfigureOptions(c);
                c.EventsType = typeof(TEvents);
            });

        builder.Services.Configure<KestrelServerOptions>(options =>
        {
            options.ConfigureHttpsDefaults(o =>
            {
                o.ClientCertificateMode = config.ClientCertificateMode;
                o.AllowAnyClientCertificate();
            });
        });

        return builder;
    }

    public static AuthenticationBuilder AddCertAuth<TEvents>(this AuthenticationBuilder builder, IConfiguration configuration, CertAuthConfig? config = null)
        where TEvents : CertificateAuthenticationEvents
    {
        builder.Services.Configure<CertAuthOptions>(configuration.GetSection(CertAuthOptions.Name));
        builder.AddInternalCertAuth<TEvents>(config);

        return builder;
    }

    public static AuthenticationBuilder AddCertAuth<TEvents>(this AuthenticationBuilder builder, CertAuthOptions configuration, CertAuthConfig? config = null)
        where TEvents : CertificateAuthenticationEvents
    {
        builder.Services.AddSingleton(Options.Options.Create(configuration));
        builder.AddInternalCertAuth<TEvents>(config);

        return builder;
    }

    public static AuthenticationBuilder AddCertAuth(this AuthenticationBuilder builder, IConfiguration configuration, CertAuthConfig? config = null) =>
        builder.AddCertAuth<DefaultCertificateEvents>(configuration, config);

    public static AuthenticationBuilder AddCertAuth(this AuthenticationBuilder builder, CertAuthOptions configuration, CertAuthConfig? config = null) =>
        builder.AddCertAuth<DefaultCertificateEvents>(configuration, config);


    public static IApplicationBuilder UseCertAuth(this IApplicationBuilder app) =>
        app.UseCertificateForwarding()
            .UseAuthentication()
            .UseAuthorization();
}