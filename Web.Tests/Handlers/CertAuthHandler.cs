using HBDStack.Web.Auths.CertAuth;
using Microsoft.Extensions.Options;

namespace Web.Tests.Handlers;

public class CertAuthHandler:DefaultCertificateEvents
{
    public CertAuthHandler(IOptions<CertAuthOptions> certAuthOptions) : base(certAuthOptions)
    {
    }
}