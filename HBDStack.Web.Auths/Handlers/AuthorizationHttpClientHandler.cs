using Microsoft.AspNetCore.Http;

namespace HBDStack.Web.Auths.Handlers;

public class AuthorizationHttpClientHandler: DelegatingHandler
{
    private readonly IHttpContextAccessor _accessor;

    public AuthorizationHttpClientHandler(IHttpContextAccessor accessor) => _accessor = accessor;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _accessor.HttpContext!.Request.Headers.Authorization.FirstOrDefault();
        request.Headers.TryAddWithoutValidation("Authorization", token);
        
        return base.SendAsync(request, cancellationToken);
    }
}