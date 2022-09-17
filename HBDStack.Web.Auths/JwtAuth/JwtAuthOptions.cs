namespace HBDStack.Web.Auths.JwtAuth;

public class JwtAuthConfig
{
    public ICollection<string> Audiences { get; set; } = new List<string>();
    public string Authority { get; set; } = default!;
    public string? MetaDataUrl { get; set; }
    public string ClientId { get; set; } = default!;
    public string? ClientSecret { get; set; }
    public bool ValidateIssuerSigningKey { get; set; } = true;
    public ICollection<string> Issuers { get; set; } = new List<string>();
    public Dictionary<string, string[]>? ClaimsValidations { get; set; }

    public bool IsMsGraphAudience()
    {
        return Authority.IsAzureAdAuthority()
               && (Audiences.Contains("00000003-0000-0000-c000-000000000000")||Audiences.Contains("https://graph.microsoft.com"));
    }
}

public class JwtAuthOptions : Dictionary<string, JwtAuthConfig>
{
    public static string Name => "Authentication:JwtAuth";
    
    public JwtAuthConfig TryGetConfig(string scheme)
    {
        if (ContainsKey(scheme)) return this[scheme];
        if (Count == 1) return Values.First();
        throw new InvalidOperationException($"The {nameof(JwtAuthConfig)} is not found.");
    }
}