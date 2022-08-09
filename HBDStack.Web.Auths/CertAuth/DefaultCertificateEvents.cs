using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.Extensions.Options;

// ReSharper disable TemplateIsNotCompileTimeConstantProblem

namespace HBDStack.Web.Auths.CertAuth;

public class DefaultCertificateEvents : CertificateAuthenticationEvents
{
    private readonly CertAuthOptions _certAuthOptions;

    public DefaultCertificateEvents(IOptions<CertAuthOptions> certAuthOptions) : this(certAuthOptions.Value)
    {
    }

    protected DefaultCertificateEvents(CertAuthOptions certAuthOptions) => _certAuthOptions = certAuthOptions;

    protected virtual (bool valid, string[]? roles) ValidateCertificate(X509Certificate2 clientCert)
    {
        foreach (var ops in _certAuthOptions)
        {
            if (!string.IsNullOrWhiteSpace(ops.Thumbprint)
                && clientCert.Thumbprint.Equals(ops.Thumbprint, StringComparison.OrdinalIgnoreCase))
                return (true, ops.Roles);

            if (!string.IsNullOrWhiteSpace(ops.CommonName))
            {
                if (clientCert.Subject.Equals(ops.CommonName, StringComparison.CurrentCultureIgnoreCase))
                    return (true, ops.Roles);

                var cn = clientCert.GetNameInfo(X509NameType.SimpleName, false);
                if (cn.Equals(ops.CommonName, StringComparison.CurrentCultureIgnoreCase))
                    return (true, ops.Roles);
            }
        }

        return (false, null);
    }

    public override async Task CertificateValidated(CertificateValidatedContext context)
    {
        var clientCert = context.ClientCertificate;

        var (valid, roles) = ValidateCertificate(clientCert);

        if (valid)
        {
            await base.CertificateValidated(context).ConfigureAwait(false);

            if (roles != null && context.Principal != null)
                context.Principal.AddIdentity(new ClaimsIdentity(roles.Select(r => new Claim(ClaimTypes.Role, r))));
        }
        else
        {
            Console.WriteLine($"Certificate {clientCert.Subject}:{clientCert.Thumbprint} is invalid.");
            context.Fail($"Certificate {clientCert.Subject} is invalid.");
        }
    }
}