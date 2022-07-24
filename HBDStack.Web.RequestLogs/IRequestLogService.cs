using HBDStack.Web.RequestLogs.Storage;
using Microsoft.EntityFrameworkCore;

namespace HBDStack.Web.RequestLogs;

public interface IRequestLogService
{
    Task<IEnumerable<LogItem>> GetLogsAsync(DateTimeOffset from, DateTimeOffset? to = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<LogItem> GetLogsAsyncEnumerable(DateTimeOffset? from=null, DateTimeOffset? to = null);
}

internal class RequestLogService : IRequestLogService
{
    private readonly IRequestLogStorage _storage;
    public RequestLogService(IRequestLogStorage storage) => _storage = storage;

    public async Task<IEnumerable<LogItem>> GetLogsAsync(DateTimeOffset from, DateTimeOffset? to = null, CancellationToken cancellationToken = default)
    {
        to ??= DateTimeOffset.Now;
        return await _storage.GetQuery().Where(i => i.Timestamp >= from && i.Timestamp <= to).ToListAsync(cancellationToken: cancellationToken);
    }

    public IAsyncEnumerable<LogItem> GetLogsAsyncEnumerable(DateTimeOffset? from=null, DateTimeOffset? to = null) => new LogAsyncEnumerable(_storage, from, to);
}