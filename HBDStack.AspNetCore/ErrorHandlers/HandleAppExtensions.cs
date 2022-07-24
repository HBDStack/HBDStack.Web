using HBDStack.AspNetCore.ErrorHandlers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

// ReSharper disable CheckNamespace

namespace Microsoft.AspNetCore.Builder;

public static class HandleAppExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app, JsonSerializerOptions jsonSerializerOptions = default, bool enableDiagnostics = false, Func<Exception, ProblemDetails> exceptionConverter = null)
    {
        if (jsonSerializerOptions != default)
            GlobalExceptionHandling.JsonSerializerOptions = jsonSerializerOptions;

        return UseGlobalExceptionHandler(app,
            namingStrategy: null,
            enableDiagnostics,
            exceptionConverter);
    }


    /// <summary>
    /// Global Handling Exception, together with transform ValidationProblemDetails by naming policy. 
    /// Ex: camelCase, snake_case or PascalCase 
    /// </summary>        
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app, NamingStrategy namingStrategy = default, bool enableDiagnostics = false, Func<Exception, ProblemDetails> exceptionConverter = null)
    {
        if (namingStrategy != default)
        {
            GlobalExceptionHandling.NamingPolicy = namingStrategy;
        }

        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                Trace.TraceError("Global exception handling...");

                var logger = LoggerFactory.Create(builder =>
                {
#if DEBUG
                    builder.AddConsole();
#endif
                    builder.AddApplicationInsights();
                }).CreateLogger(nameof(GlobalExceptionHandling));

                var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerPathFeature>();

                logger.LogError(exceptionHandlerPathFeature?.Error, exceptionHandlerPathFeature?.Error?.Message);

                if (enableDiagnostics)
                {
                    Trace.TraceError(exceptionHandlerPathFeature?.Error?.Message);
                }

                ProblemDetails problemDetails = null;
                if (exceptionConverter != null && exceptionHandlerPathFeature != null)
                    problemDetails = exceptionConverter(exceptionHandlerPathFeature.Error);

                problemDetails ??= exceptionHandlerPathFeature?.Error switch
                {
                    ArgumentNullException ex => ex.GetProblemDetails(),
                    ArgumentException ex => ex.GetProblemDetails(),
                    ValidationException ex => ex.GetProblemDetails(),
                    ApplicationException ex => ex.GetProblemDetails(),
                    AggregateException ex => ex.GetProblemDetails(),
                    Refit.ApiException ex => ex.GetProblemDetails(),
                    { } => new ProblemDetails
                    {
                        Status = HttpStatusCode.InternalServerError,
                        ErrorMessage = "An error occurred. Please try again later."
                    }
                };

                if (string.IsNullOrEmpty(problemDetails.TraceId))
                    problemDetails.TraceId = Activity.Current?.RootId ??
                                             Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

                if (problemDetails?.Status == HttpStatusCode.InternalServerError)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }

                // if (problemDetails?.Status == 0)
                // {
                //     problemDetails.Status = HttpStatusCode.BadRequest;
                // }

                context.Response.StatusCode = (int)problemDetails.Status;

                context.Response.ContentType = "application/json";

                if (enableDiagnostics)
                {
                    Trace.WriteLine(problemDetails?.ToJson(enableIndented: false));
                }

                await context.Response.WriteAsync(problemDetails?.ToJson())
                    .ConfigureAwait(false);
            });
        });

        return app;
    }
}