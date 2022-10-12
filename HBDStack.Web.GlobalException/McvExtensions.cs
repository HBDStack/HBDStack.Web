using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class McvExtensions
{
    public static IMvcBuilder AddModelStateHandler(this IMvcBuilder builder)
    {
        builder.ConfigureApiBehaviorOptions(option 
            => option.InvalidModelStateResponseFactory = 
                context =>new BadRequestObjectResult( GlobalHandlerBuilderExtensions.Handler!.ToProblemDetails(context.ModelState)));

        return builder;
    }

}