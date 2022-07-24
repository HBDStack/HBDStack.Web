using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
// ReSharper disable CheckNamespace

namespace HBDStack.AspNetCore.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
public sealed class RateLimitAttribute : ActionFilterAttribute
{
    #region Constructors

    public RateLimitAttribute(string name, int seconds = 60)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Seconds = seconds;
    }

    #endregion Constructors

    #region Properties

    public string HeaderKey { get; set; }

    public string Name { get; }

    public int Seconds { get; }

    /// <summary>
    /// Only Apply rate Limit if the request return successful status code 200-299
    /// </summary>
    public bool TrackSuccessfulOnly { get; set; }

    #endregion Properties

    #region Methods

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        base.OnActionExecuted(context);

        var key = GetKey(context.HttpContext);
        var cacher = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

        var code = context.Result is StatusCodeResult c ? c.StatusCode : context.HttpContext.Response.StatusCode;

        if (TrackSuccessfulOnly && code is >= 200 and < 300)
        {
            cacher.Set(key, true, DateTimeOffset.Now.AddSeconds(Seconds));
        }
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        var key = GetKey(context.HttpContext);
        var cacher = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

        if (cacher.TryGetValue(key, out bool _))
        {
            context.Result = new ContentResult { Content = "Too many requests." };
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        }
        else if (!TrackSuccessfulOnly)
        {
            cacher.Set(key, true, DateTimeOffset.Now.AddSeconds(Seconds));
        }
    }

    private string GetKey(HttpContext context)
    {
        var clientKey = context.Connection.RemoteIpAddress.ToString();

        if (!string.IsNullOrEmpty(HeaderKey) && context.Request.Headers.ContainsKey(HeaderKey))
            clientKey = context.Request.Headers[HeaderKey];

        return $"{Name}-{clientKey}";
    }

    #endregion Methods
}