namespace HBDStack.Web.RequestLogs;

public class RequestLoggingOptions
{
    public RequestLoggingOptions(string name) => Name = name;

    /// <summary>
    /// The Name of the Log that will be capture for request identification in database.
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// The list of request Paths that the log will be capture. If empty all the requests will be captured.
    /// </summary>
    /// <returns></returns>
    public List<string> IncludesPaths { get; set; } = new();
    
    /// <summary>
    /// The list of request Paths that the log will be ignored. If empty all the requests will be captured.
    /// </summary>
    /// <returns></returns>
    public List<string> ExcludesPaths { get; set; } = new();
    
    public string[]? CaptureHeaderKeys { get; set; }
    public bool CaptureRequestBody { get; set; } = true;
    public bool CaptureRequestQueryString { get; set; } = true;
    public bool CaptureResponseBody { get; set; } = true;
}