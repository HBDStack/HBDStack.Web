using HBD.Web.Auth.Tests.Setup;

namespace HBD.Web.Auth.Tests.Scenarios.WithAzGraph;

public class JwtAuthGraphFixture : BaseFixture
{
    private const string AuthSchema = "AzureWithGraph";
    
    public JwtAuthGraphFixture() : base(AuthSchema, new TestWebApp { OnHostCreating = () => { Environment.SetEnvironmentVariable("FeatureManagement:EnableJwtAuth", "true"); } })
    {
    }
}