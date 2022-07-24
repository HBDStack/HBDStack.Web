using HBDStack.EfCore.Relational.Helpers;
using HBDStack.Web.RequestLogs.Storage;
using Microsoft.EntityFrameworkCore;

namespace HBDStack.Web.RequestLogs.Storages.Sql.InternalStorage;

internal sealed class SqlRequestLogDbContext : DbContext
{
    private readonly SqlRequestLogOptions _config;
    private static bool _initialized;
    public SqlRequestLogDbContext(DbContextOptions<SqlRequestLogDbContext> options, SqlRequestLogOptions config) : base(options) => _config = config;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LogItem>().ToTable(_config.TableName, _config.Schema);
        modelBuilder.Entity<LogItem>().HasKey(p => p.Id);
        modelBuilder.Entity<LogItem>().Property(p => p.Name).IsRequired().HasMaxLength(50);
        modelBuilder.Entity<LogItem>().HasIndex(p => p.Name).IsUnique(false);
        modelBuilder.Entity<LogItem>().Property(p => p.Method).IsRequired().HasMaxLength(50);
        modelBuilder.Entity<LogItem>().HasIndex(p => p.Method).IsUnique(false);
        modelBuilder.Entity<LogItem>().HasIndex(p => p.UrlPath).IsUnique(false);
        modelBuilder.Entity<LogItem>().Property(p => p.UrlPath).IsRequired().HasMaxLength(1000);

        modelBuilder.Entity<LogItem>().Property(p => p.RequestQueryString).HasMaxLength(1000);
        modelBuilder.Entity<LogItem>().Property(p => p.RequestBody).HasMaxLength(4000);
        modelBuilder.Entity<LogItem>().Property(p => p.ResponseBody).HasMaxLength(4000);
        modelBuilder.Entity<LogItem>().Property(p => p.ClientIpAddress).HasMaxLength(100);
        modelBuilder.Entity<LogItem>().Property(p => p.Headers).HasConversion<DictionaryStringConverter>().HasMaxLength(2000);
    }

    
    public async Task Initialise(CancellationToken cancellationToken = default)
    {
        if(_initialized)return;
        _initialized = true;

        await this.CreateTableAsync<LogItem>(cancellationToken: cancellationToken);
    }
}