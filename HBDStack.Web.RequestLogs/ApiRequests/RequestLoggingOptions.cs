namespace HBDStack.Web.RequestLogs.ApiRequests;

public class ApiRequestLoggingOptions : RequestLoggingOptions
{
    public bool CaptureClientIpAddress { get; set; }

    public ApiRequestLoggingOptions(string name) : base(name)
    {
    }
}