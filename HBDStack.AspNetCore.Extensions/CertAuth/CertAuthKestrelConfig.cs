using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;

namespace HBDStack.AspNetCore.Extensions.CertAuth;

public static class CertAuthKestrelConfig
{
    #region Methods

    public static IWebHostBuilder ConfigureKestrelWithCertAuth(this IWebHostBuilder webHostBuilder)
        => webHostBuilder.ConfigureKestrel(options =>
            options.ConfigureHttpsDefaults(o => o.ClientCertificateMode = ClientCertificateMode.RequireCertificate));

    #endregion Methods
}