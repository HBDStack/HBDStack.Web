using HBDStack.AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

// ReSharper disable ClassNeverInstantiated.Global

namespace HBDStack.AspNetCore.Handlers;

/// <inheritdoc/>
/// <summary>
/// CRUD action validations
/// </summary>
public class ModelValidationHandler : ActionValidationHandler
{
    #region Methods

    protected override IActionResult Validate(ActionExecutingContext context)
    {
        //Ignore GET
        if (IgnoreMethod(context.HttpContext.Request))
            return null;

        //validate Model
        var rs = ValidateModelState(context);
        if (rs != null)
            return rs;

        var pathId = context.ActionArguments.ContainsKey(nameof(IModel.Id).ToLower()) ? context.ActionArguments[nameof(IModel.Id).ToLower()] : null;

        if (!(context.ActionArguments.FirstOrDefault(m => m.Value is IModel).Value is IModel model))
            return null;

        return ValidateModelIfPost(context, model) ?? ValidateModelIfPut(context, pathId, model)
            ?? ValidateModelIfDelete(context, pathId, model);
    }

    protected virtual IActionResult ValidateModelIfDelete(ActionExecutingContext context, object pathId, IModel model)
    {
        if (string.Equals(context.HttpContext.Request.Method, "DELETE", StringComparison.OrdinalIgnoreCase)
            && pathId == null)
        {
            return new BadRequestObjectResult($"Invalid DELETE Action, The Id {pathId} is invalid.");
        }
        return null;
    }

    /// <summary>
    /// When call POST method the model ID should be NULL
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual IActionResult ValidateModelIfPost(ActionExecutingContext context, IModel model)
    {
        if (string.Equals(context.HttpContext.Request.Method, "POST", StringComparison.OrdinalIgnoreCase)
            && model.Id != null)
        {
            return new BadRequestObjectResult("Invalid POST Action, The Model.Id should be empty.");
        }
        return null;
    }

    protected virtual IActionResult ValidateModelIfPut(ActionExecutingContext context, object pathId, IModel model)
    {
        if (string.Equals(context.HttpContext.Request.Method, "PUT", StringComparison.OrdinalIgnoreCase)
            && (model.Id == null || pathId == null || model.Id.ToString().Equals(pathId.ToString(), StringComparison.OrdinalIgnoreCase)))
        {
            return new BadRequestObjectResult($"Invalid PUT Action, The Model.Id {model.Id} and pathId {pathId} are not matched.");
        }
        return null;
    }

    /// <summary>
    /// Validate Model state
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual IActionResult ValidateModelState(ActionExecutingContext context)
        => !context.ModelState.IsValid ? new BadRequestObjectResult(context.ModelState) : null;

    #endregion Methods
}