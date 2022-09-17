using HBDStack.Framework.Extensions.Encryption;
using HBDStack.Web.Auths.JwtAuth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace HBD.Web.Auth.Tests.Setup;

public abstract class BaseFixture : IDisposable
{
    private readonly string _authSchema;
    private readonly string _userName = "singa-client@onepass.singa.solutions";
    private readonly string _password = "ZFVuKnFIQk56VWhlX0dhVV9XQFJBZzhvUTczWVFQanBOWA==";

    protected BaseFixture(string authSchema, TestWebApp testWebApp)
    {
        _authSchema = authSchema;
        WebApp = testWebApp;
    }

    private TestWebApp WebApp { get; }
    public IServiceProvider ServiceProvider => WebApp.ScopeServices.ServiceProvider;

    private HttpClient? _client;

    public HttpClient Client
    {
        get
        {
            _client ??= CreateClient();
            return _client;
        }
    }

    protected virtual HttpClient CreateClient()
    {
        var client = WebApp.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("xs-auth-schema", _authSchema);
        return client;
    }

    public async Task<string> GetAppToken()
    {
        var options = ServiceProvider.GetRequiredService<IOptions<JwtAuthOptions>>();
        var config = options.Value.TryGetConfig(_authSchema);

        var app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
            .WithClientSecret(config.ClientSecret)
            .WithAuthority(config.Authority)
            .WithRedirectUri($"msal{config.ClientId}://auth")
            .Build();

        var token = await app.AcquireTokenForClient(new[] { $"api://{config.ClientId}/.default" })
            .ExecuteAsync();

        return $"{token.TokenType} {token.AccessToken}";
    }

    public async Task<string> GetGraphToken()
    {
        var options = ServiceProvider.GetRequiredService<IOptions<JwtAuthOptions>>();
        var config = options.Value.TryGetConfig(_authSchema);

        var app = PublicClientApplicationBuilder.Create(config.ClientId)
            //.WithClientSecret(config.ClientSecret)
            .WithAuthority(config.Authority)
            .WithRedirectUri($"msal{config.ClientId}://auth")
            .Build();

        var token = await app.AcquireTokenByUsernamePassword(new[] { "https://graph.microsoft.com/.default" }, _userName, _password.DecryptWithBase64())
            .ExecuteAsync();

        return $"{token.TokenType} {token.AccessToken}";
    }

    public void Dispose()
    {
        WebApp.Dispose();
        GC.SuppressFinalize(this);
    }
}