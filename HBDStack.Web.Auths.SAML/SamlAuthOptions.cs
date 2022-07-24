namespace HBDStack.Web.Auths.SAML;

public class SamlAuthConfig
{
    public string EntityId { get; set; } = default!;
    public string IdentityUrl { get; set; } = default!;
    
    public string MetaDataUrl { get; set; } = default!;
}

public class SamlAuthOptions: Dictionary<string, SamlAuthConfig>
{
    public const string Name = "Authentication:Samls";
}