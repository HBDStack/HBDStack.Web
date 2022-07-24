using System.Diagnostics;
using System.Net;
using System.Text.Json;
using HBDStack.StatusGeneric;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace HBDStack.Web.GlobalException;

public interface IGlobalExceptionHandler
{
    /// <summary>
    /// Exception handler and return a <see cref="ProblemDetails"/>
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    ValueTask<ProblemDetails> HandleExceptionAsync(Exception exception);

    /// <summary>
    /// Convert IStatusGeneric to a <see cref="ProblemDetails"/>
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    ProblemDetails? ToProblemDetails(IStatusGeneric status);

    /// <summary>
    /// Convert ModelStateDictionary to a <see cref="ProblemDetails"/>
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    ProblemDetails? ToProblemDetails(ModelStateDictionary status);

    /// <summary>
    /// Serialize a <see cref="ProblemDetails"/> to string.
    /// </summary>
    /// <param name="problemDetails"></param>
    /// <returns></returns>
    string Serialize(ProblemDetails? problemDetails);
}

/// <summary>
/// The default implementation of <see cref="IGlobalExceptionHandler"/>
/// </summary>
public class DefaultGlobalExceptionHandler : IGlobalExceptionHandler
{
    public static readonly JsonSerializerOptions DefaultJsonOptions = new()
        { WriteIndented = false, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    protected ILogger<DefaultGlobalExceptionHandler> Logger { get; }
    public DefaultGlobalExceptionHandler(ILogger<DefaultGlobalExceptionHandler> logger) => Logger = logger;

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

    public virtual ProblemDetails? ToProblemDetails(IStatusGeneric status) => status.HasErrors
        ? new ProblemDetails
        {
            Status = HttpStatusCode.BadRequest,
            TraceId = Activity.Current?.RootId ?? Activity.Current?.TraceId.ToString() ?? string.Empty,
            ErrorDetails = new ProblemResultCollection(status.Errors.Select(e => e.ErrorResult))
        }
        : null;

    public virtual ProblemDetails? ToProblemDetails(ModelStateDictionary status)
    {
        if (status.IsValid) return null;

        var errorCollection = new ProblemResultCollection();

        foreach (var (key, value) in status)
        {
            errorCollection.AddRange(value.Errors.Select(i =>
                new GenericValidationResult("invalid_argument", i.ErrorMessage, new[] { key })));
        }

        return new ProblemDetails
        {
            Status = HttpStatusCode.BadRequest,
            ErrorDetails = errorCollection,
            ErrorMessage = "One or more validation errors occurred.",
            TraceId = Activity.Current?.RootId ?? Activity.Current?.TraceId.ToString() ?? null
        };
    }

    public virtual string Serialize(ProblemDetails? problemDetails) =>
        problemDetails == null ? string.Empty : JsonSerializer.Serialize(problemDetails, DefaultJsonOptions);
}