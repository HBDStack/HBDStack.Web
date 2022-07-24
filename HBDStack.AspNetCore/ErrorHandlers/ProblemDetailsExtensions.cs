using HBDStack.StatusGeneric;

namespace HBDStack.AspNetCore.ErrorHandlers;

public static class ProblemDetailsExtensions
{
    public static ProblemResult ToProblemResult(this GenericValidationResult result) =>
        new()
        {
            References = result.References,
            ErrorCode = result.ErrorCode,
            ErrorMessage = result.ErrorMessage
        };

    public static ProblemDetails ToProblemDetails(this IStatusGeneric statusGeneric) =>
        new (statusGeneric.Message)
        {
            ErrorDetails = new ProblemResultCollection(statusGeneric.Errors.Select(e => e.ErrorResult))
        };
}