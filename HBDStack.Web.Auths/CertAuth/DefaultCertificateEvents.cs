using System.Security.Claims;
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

    public override async Task CertificateValidated(CertificateValidatedContext context)
    {
        var clientCert = context.ClientCertificate;

        var validAuth = _certAuthOptions.FirstOrDefault(c =>
            !string.IsNullOrEmpty(c.Thumbprint) &&
            clientCert.Thumbprint.Equals(c.Thumbprint, StringComparison.OrdinalIgnoreCase));

        if (validAuth != null)
        {
            await base.CertificateValidated(context).ConfigureAwait(false);

            if (validAuth.Roles != null &&  context.Principal!=null)
                context.Principal.AddIdentity(new ClaimsIdentity(validAuth.Roles.Select(r => new Claim(ClaimTypes.Role, r))));
        }
        else context.Fail($"Certificate {context.ClientCertificate.Thumbprint} is invalid.");
    }
}