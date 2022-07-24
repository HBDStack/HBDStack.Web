using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HBDStack.AspNetCore.Handlers;

public abstract class ActionValidationHandler : ActionHandlerBase
{
    #region Methods

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var rs = Validate(context);
        if (rs != null)
        {
            context.Result = rs;
            return;
        }
        base.OnActionExecuting(context);
    }

    protected abstract IActionResult Validate(ActionExecutingContext context);

    #endregion Methods
}