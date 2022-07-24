namespace HBDStack.AspNetCore.Extensions.CertAuth;

public class CertAuthSetting
{
    #region Properties

    public static string Name => "CertAuth";

    /// <summary>
    /// The Name of Certificate in Key Vault.
    /// </summary>
    public IList<string> CertNames { get; } = new List<string>();

    /// <summary>
    /// Set this to validate client cert directly without loading Cert from Key Vault
    /// </summary>

    public IList<string> CertThumbprints { get; } = new List<string>();

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string VaultName { get; set; }

    #endregion Properties
}