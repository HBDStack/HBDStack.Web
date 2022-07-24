using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace HBDStack.AspNetCore.ErrorHandlers;

public static class AddErrorCodeHandlerExtensions
{
    /// <summary>
    /// Allow handle ErrorDetails when ModelState invalid
    /// </summary>
    /// <returns></returns>
    public static IMvcBuilder AddErrorCodeHandler(this IMvcBuilder builder, NamingStrategy namingStrategy = default)
    {
        builder.ConfigureApiBehaviorOptions(option 
            => option.InvalidModelStateResponseFactory = 
                context => TransformModelStateExtensions.TransformModelState(context.ModelState, namingStrategy));

        return builder;
    }
}

public static class ControllerNamingStrategyExtension
{
    /// <summary>
    /// Allow transform ErrorDetails in ProblemDetail by NamingStrategy
    /// </summary>
    public static IMvcBuilder AddErrorTransform(this IMvcBuilder builder, NamingStrategy namingStrategy = default)
    {
        if (namingStrategy == null)
            return builder;
            
        HandleControllerExtensions.NamingStrategy = namingStrategy;
        return builder;
    }
}