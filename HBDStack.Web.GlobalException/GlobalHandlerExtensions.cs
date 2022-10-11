using HBDStack.Web.GlobalException;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable CheckNamespace
namespace Microsoft.AspNetCore.Builder;

public static class GlobalHandlerBuilderExtensions
{
    public static IGlobalExceptionHandler? Handler { get; private set; }

    public static IApplicationBuilder UseGlobalExceptionHandler<THandler>(this IApplicationBuilder app) where THandler:class, IGlobalExceptionHandler
    {
        Handler ??= app.ApplicationServices.CreateInstance<THandler>();

        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;

                var problems = await Handler.HandleExceptionAsync(exception);
                var result = Handler.Serialize(problems);

                context.Response.StatusCode = (int)problems.Status;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(result).ConfigureAwait(false);
            });
        });

        return app;
    }
    
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app) 
        => app.UseGlobalExceptionHandler<DefaultGlobalExceptionHandler>();
}