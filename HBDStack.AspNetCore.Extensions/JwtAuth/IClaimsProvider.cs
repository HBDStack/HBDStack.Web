using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace HBDStack.AspNetCore.Extensions.JwtAuth;

public interface IClaimsProvider<TOptions> where TOptions : AuthenticationSchemeOptions
{
    #region Methods

    Task<ICollection<Claim>> GetClaimsAsync(ResultContext<TOptions> context);

    #endregion Methods
}

public interface IJwtClaimsProvider : IClaimsProvider<JwtBearerOptions>
{ }