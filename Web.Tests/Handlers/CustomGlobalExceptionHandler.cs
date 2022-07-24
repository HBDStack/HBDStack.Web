using HBDStack.StatusGeneric;
using HBDStack.Web.GlobalException;

namespace Web.Tests.Handlers;

public class CustomGlobalExceptionHandler : DefaultGlobalExceptionHandler
{
    public CustomGlobalExceptionHandler(ILogger<CustomGlobalExceptionHandler> logger) : base(logger)
    {
    }

    public override async ValueTask<ProblemDetails> HandleExceptionAsync(Exception exception)
    {
        var error = await base.HandleExceptionAsync(exception);
        error.TraceId = "CustomGlobalExceptionHandler";
        return error;
    }

    public override ProblemDetails? ToProblemDetails(IStatusGeneric status)
    {
        var error = base.ToProblemDetails(status);
        error!.TraceId = "CustomGlobalExceptionHandler";
        return error;
    }
}