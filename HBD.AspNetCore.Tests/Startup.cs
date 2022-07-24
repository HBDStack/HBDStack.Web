using System;
using System.Net;
using System.Text.Json.Serialization;
using HBD.AspNetCore.Tests.Exceptions;
using HBD.AspNetCore.Tests.Midlewares;
using HBDStack.AspNetCore.ErrorHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace HBD.AspNetCore.Tests;

public class Startup
{
    private readonly IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        _env = env;
    }

    public IConfiguration Configuration { get; }


    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app)
    {
        if (_env.EnvironmentName == "SnakeCaseTesting")
            app.UseGlobalExceptionHandler(enableDiagnostics: true,
                namingStrategy: new SnakeCaseNamingStrategy());
        else
            app.UseGlobalExceptionHandler(
                jsonSerializerOptions: default,
                enableDiagnostics: true,
                exceptionConverter: ExceptionConverter);

        app
            .UseRouting()
            .UseMiddleware<FakeRemoteIpAddressMiddleware>()
            .UseEndpoints(endpoints => endpoints.MapControllers());
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddMemoryCache()
            .AddRouting()
            .AddControllers();

        services
            .AddMvcCore()
            .AddJsonOptions(op =>
            {
                op.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                op.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });
    }

    private static ProblemDetails? ExceptionConverter(Exception exception) =>
        exception switch
        {
            InvalidArgumentException => new ProblemDetails()
            {
                Status = HttpStatusCode.BadRequest,
                ErrorMessage = "Invalid argument exception",
                TraceId = Guid.NewGuid().ToString()
            },
            _ => null
        };
}

internal class SnakeCaseNamingStrategy : NamingStrategy
{
    protected override string ResolvePropertyName(string name)
    {
        return name.ToSnakeCase();
    }
}