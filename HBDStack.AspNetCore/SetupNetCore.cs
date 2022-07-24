using HBDStack.AspNetCore.Handlers;
// ReSharper disable CheckNamespace

namespace Microsoft.AspNetCore.Mvc;

public static class SetupNetCore
{
    #region Methods

    public static MvcOptions AddModelHandlers(this MvcOptions options)
    {
        //options.Filters.Add<ModelUserHandler>();
        options.Filters.Add<ModelValidationHandler>();
        return options;
    }

    #endregion Methods
}