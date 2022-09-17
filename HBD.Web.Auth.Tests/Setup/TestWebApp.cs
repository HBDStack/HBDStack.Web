using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Web.Auth;

namespace HBD.Web.Auth.Tests.Setup;

public class TestWebApp : WebApplicationFactory<Program>
{
    public Action? OnHostCreating { get; set; }
    public IServiceScope ScopeServices { get; private set; } = default!;

    protected override IHostBuilder? CreateHostBuilder()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        OnHostCreating?.Invoke();
        
        return base.CreateHostBuilder();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole().AddFilter(level => level >= LogLevel.Trace);
            });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        ScopeServices = host.Services.CreateScope();
        return host;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        ScopeServices.Dispose();
    }
}