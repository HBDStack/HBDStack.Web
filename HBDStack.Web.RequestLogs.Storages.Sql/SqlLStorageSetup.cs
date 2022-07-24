using HBDStack.Web.RequestLogs.Storages.Sql.InternalStorage;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class LogSetup
{
    public static IServiceCollection AddSqlRequestStorage(this IServiceCollection service, string connectionString)
        => service.AddSqlRequestStorage(new SqlRequestLogOptions(connectionString));

    public static IServiceCollection AddSqlRequestStorage(this IServiceCollection service, SqlRequestLogOptions config)
    {
         service.AddRequestStorage<SqlRequestLogStorage>()
             .AddSingleton(config);
         
         service.AddDbContext<SqlRequestLogDbContext>(b => b
             .UseAutoTruncateStringInterceptor()
             .UseSqlServer(config.ConnectionString, o => o
                 .EnableRetryOnFailure()
                 .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
         ,ServiceLifetime.Transient,ServiceLifetime.Singleton);
         
         return service;
    }
}