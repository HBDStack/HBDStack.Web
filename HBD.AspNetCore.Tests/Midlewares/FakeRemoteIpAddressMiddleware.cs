using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace HBD.AspNetCore.Tests.Midlewares;

internal class FakeRemoteIpAddressMiddleware
{
    #region Fields

    private readonly IPAddress fakeIpAddress = IPAddress.Parse("127.0.0.1");
    private readonly RequestDelegate next;

    #endregion Fields

    #region Constructors

    public FakeRemoteIpAddressMiddleware(RequestDelegate next) => this.next = next;

    #endregion Constructors

    #region Methods

    public async Task Invoke(HttpContext httpContext)
    {
        httpContext.Connection.RemoteIpAddress = fakeIpAddress;
        await next(httpContext);
    }

    #endregion Methods
}