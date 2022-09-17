using System.Security.Claims;
using HBDStack.Web.Auths.JwtAuth;
using Microsoft.Extensions.Logging;

namespace HBDStack.Web.Auths.Providers;

public class MsGraphTokenValidator : ITokenValidator
{
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<MsGraphTokenValidator> _logger;

    public MsGraphTokenValidator(IHttpClientFactory factory, ILogger<MsGraphTokenValidator> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async ValueTask<StatusResult> ValidateAsync(string scheme, JwtAuthConfig config, ClaimsPrincipal principal)
    {
        if (!config.IsMsGraphAudience()) return StatusResult.Success();

        //Calling Graph Api
        var client = _factory.CreateClient(nameof(MsGraphTokenValidator));

        try
        {
            var rs = await client.GetStringAsync("/v1.0/me");
            return StatusResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, ex.Message);
            return StatusResult.Fails(ex.Message);
        }
    }
}