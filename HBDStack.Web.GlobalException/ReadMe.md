# HBD.AspNetCore.GlobalException

## 1. How to use it?
Before the application starts, you need to add the following code to the application startup:
```csharp
app.UseGlobalExceptionHandler();
app.Run();
```
That's it. all the exceptions within the app will be capture and response with standard problem details format as below.
```json
{
  "Status": 500,
  "TraceId": "410283d03576596b537f1f8a9e2b6cf4",
  "ErrorCode": "InternalServerError",
  "ErrorMessage": "Invalid operation",
  "ErrorDetails": {}
}
```

## 2. How to customize the handler?
If you want to customize the handler, the should provide an implementation of `IGlobalExceptionHandler`.
```csharp
public class CustomGlobalExceptionHandler : IGlobalExceptionHandler
{
    protected ILogger<DefaultGlobalExceptionHandler> Logger { get; }
    public DefaultGlobalExceptionHandler(ILogger<DefaultGlobalExceptionHandler>logger) => Logger = logger;

    public virtual ValueTask<ProblemDetails> HandleExceptionAsync(Exception exception)
    {
        var error = new ProblemDetails
        {
            Status = HttpStatusCode.InternalServerError,
            TraceId = Activity.Current?.RootId ?? Activity.Current?.TraceId.ToString() ?? string.Empty,
            ErrorMessage = exception.Message
        };

        return new ValueTask<ProblemDetails>(error);
    }

    public virtual ProblemDetails? ToProblemDetails(IStatusGeneric status)
    {
        return new ProblemDetails
        {
            Status = HttpStatusCode.InternalServerError,
            TraceId = Activity.Current?.RootId ?? Activity.Current?.TraceId.ToString() ?? string.Empty,
            ErrorDetails = new ProblemResultCollection(status.Errors.Select(e => e.ErrorResult))
        };
    }

    public virtual string Serialize(ProblemDetails? problemDetails) =>
        JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            WriteIndented = true
        });
}
```

and then add it into Global Exception handler.
```csharp
app.UseGlobalExceptionHandler<CustomGlobalExceptionHandler>();
```

Congratulations! Now the global exception will be handled by the custom handler instead of the default one.

## 3. Deal With IStatusGeneric
This library also provided some extension methods to help you to deal with `IStatusGeneric`. Please refer `ControllerExtensionsHandlerExtensions` for details.

