using HBDStack.StatusGeneric;
using Microsoft.AspNetCore.Builder;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace
namespace Microsoft.AspNetCore.Mvc;

public static class ControllerExtensionsHandlerExtensions
{
    public static ActionResult Created(this IStatusGeneric status, object result) => Send(status, result, true);

    public static ActionResult Ok(this IStatusGeneric status, object? result = null) => Send(status, result, false);

    public static ActionResult Send(this ControllerBase controller, object? result = null)
        => result == null ? controller.NoContent() : controller.Ok(result);

    public static ActionResult Send(this IStatusGeneric status, object? result = null) => Send(status, result, false);

    public static ActionResult Send(this IStatusGeneric actionStatus, object? result, bool isCreated)
    {
        if (!actionStatus.HasErrors)
        {
            if (isCreated)
                return new CreatedResult("", result);

            if (result == null)
                return new NoContentResult();

            return new JsonResult(result);
        }

        if(GlobalHandlerBuilderExtensions.Handler == null) 
            throw new InvalidOperationException("Please call UseGlobalExceptionHandler before app start to use this extension.");
        
        var error = GlobalHandlerBuilderExtensions.Handler!.ToProblemDetails(actionStatus);
        return new BadRequestObjectResult(error);
    }
}