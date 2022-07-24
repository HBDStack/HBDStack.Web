using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HBDStack.AspNetCore.Extensions.CertAuth;

/// <summary>
/// If running in localhost. Ensure the client certificate is added to Trusted Root Certification of Computer Store.
/// http://brainof-dave.blogspot.com/2008/08/remote-certificate-is-invalid-according.html
/// </summary>
public static class CertAuthConfig
{
    #region Methods

    /// <summary>
    /// Add Cert Auth Config
    /// </summary>
    /// <typeparam name="TEvents"></typeparam>
    /// <param name="services"></param>
    /// <param name="setting">If Setting is null. The setting will be import from Configuration. Ensure you call <see cref="ConfigureCertAuth"/> </param>
    /// <returns></returns>
    public static IServiceCollection AddCertAuthConfig<TEvents>(this IServiceCollection services, CertAuthSetting setting = null, CertificateTypes type = CertificateTypes.All) where TEvents : CertificateAuthenticationEvents
    {
        if (setting != null)
            services.AddSingleton(setting);

        services
            .AddScoped<TEvents>()
            .AddCertificateForwarding(cf =>
            {
                cf.CertificateHeader = "X-ARR-ClientCert";
                cf.HeaderConverter = AspCoreExtensions.ConvertBase64ToCert;
            })
            .AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
            .AddCertificate(c =>
            {
                c.AllowedCertificateTypes = type;
                c.EventsType = typeof(TEvents);
            });

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Add Cert Auth Config with settings from Configuration
    /// </summary>
    /// <typeparam name="TEvents"></typeparam>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IServiceCollection AddCertAuthConfig<TEvents>(this IServiceCollection services, IConfiguration configuration, CertificateTypes type = CertificateTypes.All) where TEvents : CertificateAuthenticationEvents
        => services.ConfigureCertAuth(configuration)
            .AddCertAuthConfig<TEvents>(type: type);

    /// <summary>
    /// Add Cert Auth Config with settings from Configuration with DefaultCertificateAuthenticationEvents
    /// </summary>
    /// <typeparam name="TEvents"></typeparam>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IServiceCollection AddCertAuthConfig(this IServiceCollection services, IConfiguration configuration, CertificateTypes type = CertificateTypes.All)
        => services.ConfigureCertAuth(configuration)
            .AddCertAuthConfig<DefaultCertificateAuthenticationEvents>(type: type);

    /// <summary>
    /// Add Cert Config with DefaultCertificateAuthenticationEvents
    /// </summary>
    /// <param name="services"></param>
    /// <param name="setting"></param>
    /// <returns></returns>
    public static IServiceCollection AddCertAuthConfig(this IServiceCollection services, CertAuthSetting setting, CertificateTypes type = CertificateTypes.All)
        => services.AddCertAuthConfig<DefaultCertificateAuthenticationEvents>(setting, type);

    /// <summary>
    /// Config IOptions<CertAuthSetting> ensure you provided the configuration section.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureCertAuth(this IServiceCollection services, IConfiguration configuration, string sectionName = null)
        => services
            .Configure<CertAuthSetting>(configuration.GetSection(sectionName ?? CertAuthSetting.Name))
            .AddSingleton(p => p.GetConfigure<CertAuthSetting>().Value);

    public static IApplicationBuilder UseCertAuth(this IApplicationBuilder app)
    {
        app.UseCertificateForwarding();
        app.UseAuthentication()
            .UseAuthorization();

        return app;
    }

    #endregion Methods
}