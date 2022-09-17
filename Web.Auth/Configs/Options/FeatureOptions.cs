namespace Web.Auth.Configs.Options;

public class FeatureOptions
{
    public static string Name => "FeatureManagement";
    
    public bool EnableJwtAuth { get; set; }
}