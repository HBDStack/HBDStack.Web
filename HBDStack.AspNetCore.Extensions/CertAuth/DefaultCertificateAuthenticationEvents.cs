using HBDStack.AzProxy.Core;
using HBDStack.AzProxy.Vault.Credentials;
using HBDStack.AzProxy.Vault.Providers;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.Extensions.Logging;

namespace HBDStack.AspNetCore.Extensions.CertAuth;

public class DefaultCertificateAuthenticationEvents : CertificateAuthenticationEvents
{
    #region Fields

    private readonly CertAuthSetting certAuthSetting;
    private readonly ILogger<DefaultCertificateAuthenticationEvents> logger;

    #endregion Fields

    #region Constructors

    public DefaultCertificateAuthenticationEvents(CertAuthSetting certAuthSetting, ILogger<DefaultCertificateAuthenticationEvents> logger)
    {
        this.certAuthSetting = certAuthSetting;
        this.logger = logger;
    }

    #endregion Constructors

    #region Methods

    public override Task AuthenticationFailed(CertificateAuthenticationFailedContext context) => base.AuthenticationFailed(context);

    public override async Task CertificateValidated(CertificateValidatedContext context)
    {
        var thumbprints = await GetThumbprintAsync();

        logger.LogWarning($"Secured with Certificates: {string.Join("|", thumbprints)} from {certAuthSetting.VaultName}");

        if (thumbprints.Any(th => context.ClientCertificate.Thumbprint.Equals(th, StringComparison.OrdinalIgnoreCase)))
        {
            await base.CertificateValidated(context).ConfigureAwait(false);
        }
        else context.Fail($"Certificate {context.ClientCertificate.Thumbprint} is invalid.");
    }

    private async ValueTask<IList<string>> GetThumbprintAsync()
    {
        if (certAuthSetting.CertThumbprints.Any())
            return certAuthSetting.CertThumbprints;

        foreach (var name in certAuthSetting.CertNames)
        {
            try
            {
                using var loader = new VaultCertProvider(name,
                    new VaultInfo { VaultName = certAuthSetting.VaultName },
                    new AppClientCredentials(certAuthSetting.ClientId, certAuthSetting.ClientSecret)
                );

                using var cert = await loader.GetCertAsync().ConfigureAwait(false);

                if (cert != null)
                    certAuthSetting.CertThumbprints.Add(cert.Thumbprint);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to load: {name} from {certAuthSetting.VaultName}");
            }
        }

        return certAuthSetting.CertThumbprints;
    }

    #endregion Methods
}