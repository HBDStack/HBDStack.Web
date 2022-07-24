using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using HBDStack.StatusGeneric;

[assembly: InternalsVisibleTo("HBDStack.AspNetCore.Tests")]

namespace HBDStack.AspNetCore.ErrorHandlers;

/// <summary>
/// Handle global exception. There are application exception will be handle if the message matches:
/// concurrency
/// </summary>
public static class GlobalExceptionHandling
{
    // Todo: replace Newtonsoft with Json text later
    public static NamingStrategy NamingPolicy = new CamelCaseNamingStrategy();

    public static JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private const string CONCURRENCY_ERROR = "concurrency";
    private const string INVALID_ARGUMENT = "invalid_argument";
    private const string ARGUMENT_ERROR_MESSAGGE = "One or more validation errors occurred.";

    public const string CONCURRENCY_ERROR_MESSAGE =
        "An concurrent error occur when execute this request. Please retry again";

    public const string INTERNALE_ERROR_MESSAGE = "An error occurred. Please try again later.";

    internal static ProblemDetails GetProblemDetails(this ArgumentNullException argumentNullException)
    {
        if (argumentNullException is ArgumentException argument)
            return argument.GetProblemDetails();

        return new ProblemDetails
        {
            Status = HttpStatusCode.BadRequest,
            TraceId = "",
            ErrorMessage = argumentNullException?.Message
        };
    }

    internal static ProblemDetails GetProblemDetails(this ArgumentException argumentException)
    {
        if (argumentException == null) throw new ArgumentNullException(nameof(argumentException));

        var error = new ProblemDetails
        {
            Status = HttpStatusCode.BadRequest,
            TraceId = "",
            ErrorMessage = argumentException.Message
        };

        if (string.IsNullOrEmpty(argumentException.ParamName))
        {
            error.ErrorMessage = argumentException.Message;
        }
        else
        {
            error.ErrorDetails = new ProblemResultCollection
            {
                new GenericValidationResult(INVALID_ARGUMENT,
                    argumentException.Message,
                    new[] { argumentException.ParamName })
            };
            error.ErrorMessage = ARGUMENT_ERROR_MESSAGGE;
        }

        return error;
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception.message?view=netcore-3.1
    /// The message string appended with the name of the invalid parameter. 
    /// this method will find out the origin message
    /// </summary>
    private static string ExcludingParameterFor(string argumentExceptionMessage)
    {
        if (string.IsNullOrEmpty(argumentExceptionMessage)) return string.Empty;
        return argumentExceptionMessage.IndexOf(' ') > 0
            ? argumentExceptionMessage[..argumentExceptionMessage.IndexOf(' ')]
            : argumentExceptionMessage;
    }

    internal static ProblemDetails GetProblemDetails(this ValidationException validationException)
    {
        var error = new ProblemDetails
        {
            Status = HttpStatusCode.BadRequest,
            TraceId = "",
            ErrorMessage = validationException.Message
        };

        if (TryParseProblemDetail(validationException.Message, out var problem))
        {
            if (problem?.Status == 0)
            {
                problem.Status = HttpStatusCode.BadRequest;
            }

            return problem;
        }

        error.ErrorDetails = new ProblemResultCollection();
        foreach (var member in validationException.ValidationResult?.MemberNames)
        {
            error.ErrorDetails.Add(new GenericValidationResult(INVALID_ARGUMENT,
                validationException.ValidationResult.ErrorMessage,
                new[] { member }));
            error.ErrorMessage = ARGUMENT_ERROR_MESSAGGE;
        }

        return error;
    }

    private static bool TryParseProblemDetail(string message, out ProblemDetails problem)
    {
        try
        {
            problem = JsonConvert.DeserializeObject<ProblemDetails>(message);
            //TODO: Why need to check this?
            return !string.IsNullOrEmpty(problem.ErrorCode);
        }
        catch
        {
            problem = null;
            return false;
        }
    }

    internal static ProblemDetails GetProblemDetails(this AggregateException aggregateException)
    {
        var error = new ProblemDetails
        {
            Status = HttpStatusCode.BadRequest,
            TraceId = "",
            ErrorMessage = aggregateException.Message
        };

        var validationExceptions = aggregateException.InnerExceptions?
            .Where(ex => ex is ValidationException)
            .OfType<ValidationException>().ToList();

        if (!validationExceptions.Any())
        {
            return error;
        }

        error.ErrorDetails = new ProblemResultCollection();
        foreach (var exception in validationExceptions)
        {
            error.ErrorDetails.Add(new GenericValidationResult(exception.ValidationResult));

            //No need to group the error message by member anymore this shall be return to client directly.
            //var (members, errorMessage) = exception.ValidationResult.GetError();
            // foreach (var member in members)
            // {
            //     var errorList = new List<string>();
            //     errorList.Add(errorMessage);
            //     
            //     var isExist = error.ErrorDetails.TryGetValue(member, out string[] errorMessages);
            //     if (isExist)
            //     {
            //         error.ErrorDetails.Remove(member);
            //         foreach (var msg in errorMessages)
            //         {
            //             errorList.Add(msg);
            //         }
            //     }
            //     
            //     error.ErrorDetails.Add(member, errorList.Distinct().ToArray());
            // }
        }

        if (error.ErrorMessage?.Any() == true)
        {
            error.ErrorMessage = ARGUMENT_ERROR_MESSAGGE;
        }

        return error;
    }

    internal static ProblemDetails GetProblemDetails(this ApplicationException validationException)
    {
        var error = new ProblemDetails
        {
            Status = HttpStatusCode.BadRequest,
            TraceId = "",
            ErrorMessage = validationException.Message
        };

        if (validationException.Message == CONCURRENCY_ERROR)
        {
            error.ErrorMessage = CONCURRENCY_ERROR_MESSAGE;
            error.Status = HttpStatusCode.Conflict;
        }

        return error;
    }

    internal static ProblemDetails GetProblemDetails(this Refit.ApiException apiException)
    {
        if (apiException.StatusCode == HttpStatusCode.BadRequest)
        {
            try
            {
                ProblemDetails problem;
                if (apiException is Refit.ValidationApiException exception)
                {
                    var validationProblem = exception.Content;
                    if (validationProblem == null)
                    {
                        throw new ArgumentException("Cannot handle problem detail from ApiException");
                    }

                    problem = new ProblemDetails
                    {
                        Status = HttpStatusCode.BadRequest,
                        ErrorMessage = validationProblem.Detail
                    };

                    if (!validationProblem.Errors.Any()) return problem;

                    problem.ErrorDetails = new ProblemResultCollection();
                    foreach (var (key, value) in validationProblem.Errors)
                    foreach (var errorMessage in value)
                        problem.ErrorDetails.Add(new GenericValidationResult(errorMessage, new[] { key }));
                }
                else
                {
                    problem = JsonConvert.DeserializeObject<ProblemDetails>(apiException.Content) ??
                              new ProblemDetails(apiException.Message)
                              {
                                  Status = apiException.StatusCode
                              };
                }

                return problem;
            }
            catch
            {
                Trace.TraceError(
                    $"Error when trying to convert ValidationProblemDetail from upstream. Content:{apiException.Content}");
            }
        }

        switch (apiException.StatusCode)
        {
            case HttpStatusCode.NotFound:
                return new ProblemDetails { Status = HttpStatusCode.NotFound, ErrorMessage = "Resource not found" };
            case HttpStatusCode.TooManyRequests:
                return new ProblemDetails
                {
                    Status = HttpStatusCode.TooManyRequests,
                    ErrorMessage = "You're reached the maximum request threshold. Please retry later."
                };
            default:
                var problem = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(apiException.Content,
                    JsonSerializerOptions);
                if (problem != null && problem.Status == apiException.StatusCode)
                    return problem;
                return new ProblemDetails
                {
                    Status = apiException.StatusCode, ErrorMessage = apiException.Content
                };
        }
    }

    public static string ToJson(this ProblemDetails problemDetails, bool enableIndented = false)
    {
        if (problemDetails == null) return string.Empty;

        var selfType = problemDetails.GetType();
        Debug.WriteLine($"Self type: {selfType}");

        //var problemAsTransformed = TransformToNamingPolicy(problemDetails, NamingPolicy);

        if (enableIndented)
        {
            return JsonConvert.SerializeObject(problemDetails, selfType, Formatting.Indented, null);
        }

        if (NamingPolicy is null or CamelCaseNamingStrategy)
            return System.Text.Json.JsonSerializer.Serialize(problemDetails, selfType, JsonSerializerOptions);

        // Todo: replace Newtonsoft with Json text later
        return JsonConvert.SerializeObject(problemDetails, selfType, Formatting.None, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = NamingPolicy
            }
        });
    }

    // private static ProblemDetails TransformToNamingPolicy(ProblemDetails problemDetails, NamingStrategy namingPolicy)
    // {
    //     if (problemDetails == null)
    //     {
    //         Trace.TraceError($"Figure out a ProblemDetails which cannot do transform to NamingPolicy");
    //         return null;
    //     }
    //
    //     if (problemDetails.ErrorDetails == null || problemDetails.ErrorDetails.Count == 0)
    //     {
    //         Debug.WriteLine($"No Error Detail to transform");
    //         return problemDetails;
    //     }
    //
    //     var errors = new Dictionary<string, string[]>();
    //
    //     foreach (var item in problemDetails.ErrorDetails)
    //     {
    //         errors.Add(namingPolicy.GetPropertyName(item.Key, false), item.Value);
    //     }
    //
    //     problemDetails.ErrorDetails = errors;
    //
    //     return problemDetails;
    // }

    // private static (List<string> members, string errorMessage) GetError(this ValidationResult validationResult)
    // {
    //     var members = new List<string>();
    //     if (!validationResult.MemberNames.Any())
    //     {
    //         return (members, string.Empty);
    //     }
    //
    //     members.AddRange(validationResult.MemberNames);
    //
    //     return (members, validationResult.ErrorMessage);
    // }
}