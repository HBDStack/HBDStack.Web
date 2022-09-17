using HBD.Web.Auth.Tests.Setup;

namespace HBD.Web.Auth.Tests.Scenarios.WithAdApp;

public class JwtAuthFixture : BaseFixture
{
    private const string AuthSchema = "Azure";
    
    public JwtAuthFixture() : base(AuthSchema, new TestWebApp { OnHostCreating = () => { Environment.SetEnvironmentVariable("FeatureManagement:EnableJwtAuth", "true"); } })
    {
    }
}