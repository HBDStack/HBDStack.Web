using System.Security.Claims;
using HBDStack.Web.Auths.JwtAuth;

namespace HBDStack.Web.Auths.Providers;

public interface ITokenValidator
{
   ValueTask<StatusResult> ValidateAsync(string scheme,JwtAuthConfig config,  ClaimsPrincipal principal);
}