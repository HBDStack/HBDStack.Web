using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace HBDStack.AspNetCore.Extensions.Swagger;

internal class SwaggerAuthorizedMiddleware
{
    #region Fields

    private readonly RequestDelegate _next;

    #endregion Fields

    #region Constructors

    /// <summary>
    ///
    /// </summary>
    /// <param name="next"></param>
    public SwaggerAuthorizedMiddleware(RequestDelegate next) => _next = next;

    #endregion Constructors

    #region Methods

    /// <summary>
    ///
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger")
            && !context.User.Identity.IsAuthenticated)
        {
            await context.ChallengeAsync();

            //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //return;
        }

        await _next.Invoke(context);
    }

    #endregion Methods
}