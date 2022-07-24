using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace HBDStack.Web.Auths.OpenID;

public class OpenIdConfig
{
    public string Authority { get; set; } = default!;
    public string? MetaDataUrl { get; set; }
    public string ClientId { get; set; } = default!;
    public string? ClientSecret { get; set; }
    public string ResponseType { get; set; } = OpenIdConnectResponseType.Code;
    public string ResponseMode { get; set; }  =OpenIdConnectResponseMode.Query;
    public bool GetClaimsFromUserInfoEndpoint { get; set; }
    public string[]? Scopes { get; set; }
}

public class OpenIdOptions : Dictionary<string, OpenIdConfig>
{
    public const string Name = "Authentication:OpenID";
}