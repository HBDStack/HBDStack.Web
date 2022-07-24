using HBDStack.Web.RequestLogs;
using HBDStack.Web.RequestLogs.HttpClients;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace HBD.Web.Logs.Storages.Sql.Tests;

public class ApiFixture : WebApplicationFactory<Program>
{
    public IServiceScope ScopeServices { get; private set; } = default!;

    protected override IHostBuilder? CreateHostBuilder()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        return base.CreateHostBuilder();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(service =>
        {
            service.AddSingleton(Options.Create(new RequestLoggingOptions("ClientLogging")
            {
            })).AddTransient<RequestLogClientHandler>();
        });

        // builder.ConfigureServices(services =>
        // {
        //     if (!TestWithSqlServer)
        //     {
        //         //services.Remove<AuthContext>().Remove<DbContextOptions<AuthContext>>();
        //         services.Remove<SingaContext>().Remove<DbContextOptions<SingaContext>>();
        //
        //         //Use InMemory
        //         // services
        //         //     .AddDbContext<AuthContext>(b => b
        //         //         .ConfigureWarnings(w => w.Log(CoreEventId.ManyServiceProvidersCreatedWarning))
        //         //         .UseInMemoryDatabase(nameof(AuthContext)));
        //         services
        //             .AddDbContext<SingaContext>(b =>
        //                 b.ConfigureWarnings(w => w.Log(CoreEventId.ManyServiceProvidersCreatedWarning))
        //                     .UseAutoConfigModel(o =>
        //                         o.ScanFrom(typeof(InfraSetup).Assembly, typeof(DomainSchemas).Assembly))
        //                     .UseInMemoryDatabase(nameof(SingaContext)));
        //     }
        // });
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
        ScopeServices?.Dispose();
    }
}