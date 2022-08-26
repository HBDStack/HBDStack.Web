using System.Diagnostics;

namespace HBDStack.AspNetCore.Extensions.JwtAuth;

[DebuggerDisplay("Authority: {Authority}")]
public class JwtAuthSetting
{
    #region Properties

    /// <summary>
    /// The AppSettings section name
    /// </summary>
    public static string Name => "AzureAd";

    public IList<JwtAppAuth> Apps { get; } = new List<JwtAppAuth>();

    /// <summary>
    /// The combination of "{BaseUrl}/{Tenant ?? TenantId}"
    /// Or "{BaseUrl}/tfp/{Tenant ?? TenantId}/{Policy}/v2.0" if <see cref="SignInPolicy"/> is provided
    /// </summary>
    public virtual string Authority
        => string.IsNullOrWhiteSpace(SignInPolicy) ? $"{BaseUrl}/{Tenant ?? TenantId}/" : $"{BaseUrl}/tfp/{Tenant ?? TenantId}/{SignInPolicy}/{Version}";

    /// <summary>
    /// The Base URL of Identity default is https://login.microsoftonline.com
    /// </summary>
    public virtual string BaseUrl { get; set; } = "https://login.microsoftonline.com";

    public virtual IList<string> Issuers => new List<string> { Authority, $"{BaseUrl}/{TenantId ?? Tenant}/", $"{BaseUrl}/tfp/{TenantId ?? Tenant}/${Version}/" };

    /// <summary>
    /// To Support Azure BC2 Signin Policy
    /// </summary>
    public string SignInPolicy { get; set; }

    public string Tenant { get; set; }

    public string TenantId { get; set; }

    /// <summary>
    /// The Signin version of Azure B2C. Default is v2.0.
    /// </summary>
    public string Version { get; set; } = "v2.0";

    #endregion Properties

    #region Methods

    public virtual JwtAppAuth GetApp(string name)
    {
        var config = Apps.FirstOrDefault(a => a.Name.EndsWith(name, StringComparison.CurrentCultureIgnoreCase));
        if (config == null) return null;

        config.Authority = Authority;
        config.Issuers = Issuers;
        config.Tenant = Tenant ?? TenantId;
        config.TenantId = TenantId ?? Tenant;
        config.IsB2cApp = !string.IsNullOrEmpty(SignInPolicy);

        if (!config.Audiences.Contains(config.ClientId))
            config.Audiences.Add(config.ClientId);

        return config;
    }

    #endregion Methods
}