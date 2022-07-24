using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace HBDStack.Web.Auths;

public class AuthsOptions
{
    /// <summary>
    /// Must provide this forwarder if there are more then 1 schemes registered.
    /// </summary>
    public Func<HttpContext, string>? DefaultSchemeForwarder { get; set; }
    public Action<AuthorizationOptions>? DefaultAuthPolicyBuilder { get; set; }
    public Action<AuthenticationOptions>? AuthenticationOptionsBuilder { get; set; }
}