using System.Diagnostics;
using HBDStack.Web.RequestLogs.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HBDStack.Web.RequestLogs.ApiRequests;

public class ApiRequestLogMiddleware
{
    private ApiRequestLoggingOptions _options = default!;
    private readonly RequestDelegate _next;

    public ApiRequestLogMiddleware(RequestDelegate next) => _next = next;

    private bool ShouldLogging(string path)
    {
        var excluded = _options!.ExcludesPaths.Any(c => path.Equals(c, StringComparison.CurrentCultureIgnoreCase));
        if (excluded) return false;

        return !_options!.IncludesPaths.Any() || _options.IncludesPaths.Any(c => path.Contains(c) || c.Contains(path));
    }

    private async Task<string> RunAndResponseAsync(HttpContext context)
    {
        var originalBody = context.Response.Body;

        try
        {
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            await _next(context);

            memStream.Position = 0;
            var responseBody = await new StreamReader(memStream).ReadToEndAsync();

            memStream.Position = 0;
            await memStream.CopyToAsync(originalBody);

            return responseBody;
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    protected virtual async Task LogAsync(HttpContext context, string id, string? requestBody, string? responseBody)
    {
        try
        {
            var item = new LogItem(id, _options!.Name, context.Request.Path.Value,context.Request.Method, context.Response.StatusCode)
                { RequestBody = requestBody, ResponseBody = responseBody };
            //Client IP
            if (_options!.CaptureClientIpAddress)
                item.ClientIpAddress = context.Connection.RemoteIpAddress.ToString();
            //QueryString
            if (_options.CaptureRequestQueryString)
                item.RequestQueryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null;
            //Headers
            if (_options.CaptureHeaderKeys?.Any() == true)
                foreach (var headerKey in _options.CaptureHeaderKeys)
                    if (context.Request.Headers.TryGetValue(headerKey, out var value))
                        item.Headers.TryAdd(headerKey, value);

            var storage = context.RequestServices.GetRequiredService<IRequestLogStorage>();
            await storage.LogAsync(item);
        }
        catch (Exception ex)
        {
            var log = context.RequestServices.GetRequiredService<ILogger<ApiRequestLogMiddleware>>();
            log.LogError(ex, ex.Message);
        }
    }

    public async Task InvokeAsync(HttpContext context)
    {
        //Prepared the parameters
        _options = context.RequestServices.GetRequiredService<IOptions<ApiRequestLoggingOptions>>().Value;

        var path = context.Request.Path.Value;
        if (!ShouldLogging(path))
        {
            await _next(context);
            return;
        }

        var id = Activity.Current?.RootId ?? Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        var requestBody = _options.CaptureRequestBody ? await context.Request.PeekBodyAsync() : null;
        var responseBody = string.Empty;

        if (_options.CaptureResponseBody)
            requestBody = await RunAndResponseAsync(context);
        else await _next(context);

        await LogAsync(context, id, requestBody, responseBody);
    }
}