using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using HBDStack.AspNetCore.ErrorHandlers;
using HBDStack.StatusGeneric;

// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global

namespace Microsoft.AspNetCore.Mvc;

public static class HandleControllerExtensions
{
    public static NamingStrategy NamingStrategy { get; set; } = new CamelCaseNamingStrategy();

    public static ActionResult Created(this IStatusGeneric status, object result) => Send(status, result, true);

    public static ActionResult Ok(this IStatusGeneric status, object result = null) => Send(status, result, false);

    public static ActionResult Send(this ControllerBase controller, object result = null)
        => result == null ? controller.NoContent() : controller.Ok(result);

    public static ActionResult Send(this IStatusGeneric status, object result = null) =>
        Send(status, result, false);

    public static ActionResult Send(this IStatusGeneric actionStatus, object result, bool isCreated)
    {
        // var activity = new Activity("snake transforms status");
        // activity.Start();

        if (!actionStatus.HasErrors)
        {
            if (isCreated)
                return new CreatedResult("", result);

            if (result == null)
                return new NoContentResult();

            return new JsonResult(result);
        }

        // var errorCode = (string.IsNullOrEmpty(actionStatus?.ErrorCode)
        //                     ? INVALID_ARGUMENT : actionStatus?.ErrorCode)
        //                 .ToLowerInvariant();
        //
        // var errorMessage = string.IsNullOrEmpty(actionStatus?.ErrorMessage)
        //                         ? "One or more validation errors occurred." : actionStatus?.ErrorMessage;

        var error = actionStatus.ToProblemDetails();
        error.TraceId = Activity.Current?.RootId ?? Activity.Current?.TraceId.ToString() ?? string.Empty;

        // var error = new HBDStack.AspNetCore.ErrorHandlers.ProblemDetails
        // {
        //     TraceId = Activity.Current?.RootId ?? Activity.Current?.TraceId.ToString() ?? string.Empty,
        //     ErrorMessage = actionStatus.Message
        // };
        //
        // if (!actionStatus.Errors.Any())
        // {
        //     return new BadRequestObjectResult(error);
        // }

        // if (actionStatus.Errors.Any(e => !(e.ErrorResult.MemberNames).Any()))
        // {
        //     throw new InvalidOperationException("Expect MemberNames must have values when ErrorCode and ErrorMessage not specify.");
        // }

        // var errorDetails = new ProblemResultCollection();
        // errorDetails.AddRange(actionStatus.Errors.Select(e => e.ErrorResult));

        // foreach (var errorMember in actionStatus.Errors)
        // {
        //     Debug.WriteLine($"Members:{errorMember.ErrorResult?.MemberNames?.Count()} ==> { errorMember.ErrorResult.ErrorMessage}");
        //     if (errorMember.ErrorResult?.MemberNames?.Count() == 0)
        //     {
        //         continue;
        //     }
        //
        //     foreach (var member in errorMember.ErrorResult.MemberNames)
        //     {
        //         var key = namingStrategy.GetPropertyName(member, false);
        //         Debug.WriteLine($"field:{key}");
        //         if (errorDetails.ContainsKey(key))
        //         {
        //             var value = errorDetails[key];
        //             if (value?.Contains(errorMember.ErrorResult?.ErrorMessage) == true)
        //             {
        //                 continue;
        //             }
        //
        //             Array.Resize(ref value, value.Length + 1);
        //             value[^1] = errorMember.ErrorResult.ErrorMessage;
        //             errorDetails[key] = value;
        //         }
        //         else
        //         {
        //             errorDetails.Add(key, new string[] { errorMember.ErrorResult.ErrorMessage });
        //         }
        //     }
        // }

        //error.ErrorDetails = errorDetails;

        // activity.Stop();
        // Debug.WriteLine($"Transform: {activity.Duration.TotalMilliseconds}ms");

        return new BadRequestObjectResult(error);
    }
}