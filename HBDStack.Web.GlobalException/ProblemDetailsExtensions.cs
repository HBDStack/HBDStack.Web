using HBDStack.StatusGeneric;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HBDStack.Web.GlobalException;

public static class ProblemDetailsExtensions
{
    public static ProblemResult ToProblemResult(this GenericValidationResult result) =>
        new()
        {
            References = result.References,
            ErrorCode = result.ErrorCode,
            ErrorMessage = result.ErrorMessage
        };


    public static ProblemDetails? ToProblemDetails(this ModelStateDictionary state)
        => GlobalHandlerBuilderExtensions.Handler?.ToProblemDetails(state);

}