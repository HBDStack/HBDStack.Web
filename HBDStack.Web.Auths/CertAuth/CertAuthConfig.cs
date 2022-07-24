using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Https;

namespace HBDStack.Web.Auths.CertAuth;

public class CertAuthConfig
{
    /// <summary>
    /// Default Scheme is <see cref="CertificateAuthenticationDefaults.AuthenticationScheme"/>
    /// </summary>
    public string? Scheme { get; set; }
    public string CertificateForwardingHeader { get; set; } = "X-ARR-ClientCert";
    public ClientCertificateMode ClientCertificateMode { get; set; } = ClientCertificateMode.RequireCertificate;
    public Action<CertificateAuthenticationOptions> ConfigureOptions { get; set; } = option =>
    {
        option.ValidateCertificateUse = true;
        option.ValidateValidityPeriod = true;
                
        option.AllowedCertificateTypes = CertificateTypes.All;
    };
}