namespace HBDStack.AspNetCore.Extensions.JwtAuth;

public class JwtAppAuth
{
    #region Properties

    /// <summary>
    /// The adition Audiences the CLientId will be included by default.
    /// </summary>
    public IList<string> Audiences { get; set; } = new List<string>();

    /// <summary>
    /// Authority Url
    /// </summary>
    public string Authority { get; set; }

    public string ClientCert { get; set; }

    public string ClientCertPass { get; set; }

    /// <summary>
    /// ClientId or Audience
    /// </summary>
    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public bool IsB2cApp { get; set; }

    public ICollection<string> Issuers { get; set; }

    public string Name { get; set; }

    public AuthOptions OverrideOptions { get; set; }

    public string Tenant { get; set; }

    public string TenantId { get; set; }

    #endregion Properties
}