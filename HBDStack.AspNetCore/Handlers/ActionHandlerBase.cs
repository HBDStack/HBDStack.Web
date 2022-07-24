using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HBDStack.AspNetCore.Handlers;

public abstract class ActionHandlerBase : ActionFilterAttribute
{
    #region Methods

    protected virtual bool IgnoreMethod(HttpRequest request)
        => string.Equals(request.Method, "GET", StringComparison.OrdinalIgnoreCase);

    #endregion Methods
}