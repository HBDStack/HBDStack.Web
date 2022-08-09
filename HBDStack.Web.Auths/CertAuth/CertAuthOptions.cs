// ReSharper disable CollectionNeverUpdated.Global

namespace HBDStack.Web.Auths.CertAuth;

public class CertAuthItem
{
    public string? Thumbprint { get; set; }
    public string? CommonName { get; set; }
    public string[]? Roles { get; set; }
}

public class CertAuthOptions : List<CertAuthItem>
{
    public static string Name => "Authentication:CertAuth";
}