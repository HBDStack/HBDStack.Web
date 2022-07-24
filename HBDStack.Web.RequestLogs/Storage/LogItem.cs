namespace HBDStack.Web.RequestLogs.Storage;

public class LogItem
{
    public LogItem(string id, string name, string urlPath, string method, int statusCode)
    {
        Id = id;
        UrlPath = urlPath;
        StatusCode = statusCode;
        Method = method;
        Name = name;
        Timestamp = DateTimeOffset.Now;
    }

    public string Id { get; private set; }
    public string Name { get; private set; }
    public string UrlPath { get; private set; }
    public string Method { get; private set; }
    public int StatusCode { get; private set; }
    public string? ClientIpAddress { get; set; }
    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public string? RequestQueryString { get; set; }
    public DateTimeOffset Timestamp { get; private set; }
}