using System.Runtime.InteropServices;
using HBDStack.Web.RequestLogs.Storages.Sql.InternalStorage;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace HBD.Web.Logs.Storages.Sql.Tests;

internal class SqlStateContextFactory : IDesignTimeDbContextFactory<SqlRequestLogDbContext>
{
    public static string ConnectionString =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? $"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={nameof(SqlRequestLogDbContext)};Integrated Security=True;Connect Timeout=30;"
            : $"Data Source=192.168.1.95;Initial Catalog={nameof(SqlRequestLogDbContext)};User Id=sa;Password=Pass@word1;";
    
    public SqlRequestLogDbContext CreateDbContext(string[] args)
    {
        var service = new ServiceCollection()
            .AddSqlRequestStorage(ConnectionString)
            .BuildServiceProvider();

        return service.GetRequiredService<SqlRequestLogDbContext>();
    }
}