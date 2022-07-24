using System.Diagnostics;
using HBDStack.Web.RequestLogs.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HBDStack.Web.RequestLogs.HttpClients;

public class RequestLogClientHandler : DelegatingHandler
{
    private const string RequestIdHeaderName = "Request-Id";
    private readonly IRequestLogStorage _storage;
    private readonly ILogger<RequestLogClientHandler> _logger;
    private readonly RequestLoggingOptions _options;

    public RequestLogClientHandler(IOptions<RequestLoggingOptions> options, IRequestLogStorage storage, ILogger<RequestLogClientHandler> logger)
    {
        _storage = storage;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //Add Request Header for tracing purposes.
        var id = Activity.Current?.RootId ?? Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();
        if (!request.Headers.Contains(RequestIdHeaderName))
            request.Headers.TryAddWithoutValidation(RequestIdHeaderName, id);

        var response = await base.SendAsync(request, cancellationToken);

        if (ShouldLogging(request.RequestUri!.AbsoluteUri))
            await LogAsync(id, response);

        return response;
    }

    private bool ShouldLogging(string path)
    {
        var excluded = _options!.ExcludesPaths.Any(c => path.Equals(c, StringComparison.CurrentCultureIgnoreCase));
        if (excluded) return false;

        return !_options!.IncludesPaths.Any() || _options.IncludesPaths.Any(c => path.Contains(c) || c.Contains(path));
    }

    protected virtual async Task LogAsync(string id, HttpResponseMessage response)
    {
        try
        {
            var path = response.RequestMessage!.RequestUri!.AbsoluteUri;
            var method = response.RequestMessage!.Method.Method;
            var status = (int)response.StatusCode;

            var item = new LogItem(id, _options.Name, path, method, status);

            //Request
            if (_options.CaptureRequestBody && response.RequestMessage.Content != null)
                item.RequestBody = await response.RequestMessage.Content.ReadAsStringAsync();

            //Response
            if (_options.CaptureResponseBody)
                item.ResponseBody = await response.Content.ReadAsStringAsync();

            //QueryString
            if (_options.CaptureRequestQueryString)
                item.RequestQueryString = response.RequestMessage.RequestUri.Query;
            //Headers
            if (_options.CaptureHeaderKeys?.Any() == true)
                foreach (var headerKey in _options.CaptureHeaderKeys)
                    if (response.RequestMessage.Headers.TryGetValues(headerKey, out var value))
                        item.Headers.TryAdd(headerKey, value.First());

            await _storage.LogAsync(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}