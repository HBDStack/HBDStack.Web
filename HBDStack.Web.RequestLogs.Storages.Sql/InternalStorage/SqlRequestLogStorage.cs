using HBDStack.Web.RequestLogs.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HBDStack.Web.RequestLogs.Storages.Sql.InternalStorage;

internal sealed class SqlRequestLogStorage : IRequestLogStorage,IDisposable
{
    private readonly IServiceScope _provider;
    private SqlRequestLogDbContext? _readerDbContext;
    public SqlRequestLogStorage(IServiceProvider provider) => _provider = provider.CreateScope();
    
    /// <summary>
    /// Todo: Enhance this method to push to a Queue and a background job to store items to the Db by batch.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    public async Task LogAsync(LogItem item, CancellationToken cancellationToken = default)
    {
        await using var db = _provider.ServiceProvider.GetRequiredService<SqlRequestLogDbContext>();
        await db.Initialise( cancellationToken);
        
        await db.Set<LogItem>().AddAsync(item, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<LogItem> GetQuery()
    {
        _readerDbContext ??= _provider.ServiceProvider.GetRequiredService<SqlRequestLogDbContext>();
        return _readerDbContext.Set<LogItem>().AsNoTracking();
    }

    public void Dispose()
    {
        _provider.Dispose();
        _readerDbContext?.Dispose();
    }
}