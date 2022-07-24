using HBDStack.Web.RequestLogs.Storage;
using Microsoft.EntityFrameworkCore;

namespace HBDStack.Web.RequestLogs;

public class LogAsyncEnumerable : IAsyncEnumerable<LogItem>
{
    private readonly IRequestLogStorage _storage;
    private readonly DateTimeOffset? _from;
    private readonly DateTimeOffset? _to;

    public LogAsyncEnumerable(IRequestLogStorage storage, DateTimeOffset? from, DateTimeOffset? to)
    {
        _storage = storage;
        _from = from;
        _to = to;
    }

    public IAsyncEnumerator<LogItem> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new LogItemAsyncEnumerator(_storage, _from, _to);
}

internal class LogItemAsyncEnumerator : IAsyncEnumerator<LogItem>
{
    private readonly IRequestLogStorage _storage;
    private readonly DateTimeOffset? _from;
    private readonly DateTimeOffset? _to;
    private const int PageSize = 100;
    private int _nextPagePage = 0;
    private int _currentItemIndex = -1;
    private bool _hasNextPage = true;
    private readonly List<LogItem> _currentList = new();

    public LogItemAsyncEnumerator(IRequestLogStorage storage, DateTimeOffset? from, DateTimeOffset? to)
    {
        _storage = storage;
        _from = from;
        _to = to;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private IQueryable<LogItem> GetQuery()
    {
        var query = _storage.GetQuery();
        if (_from.HasValue)
        {
            var f = _from.Value;
            query = query.Where(l => l.Timestamp >= f);
        }

        if (_to.HasValue)
        {
            var t = _to.Value;
            query = query.Where(l => l.Timestamp <= t);
        }

        return query.OrderByDescending(l => l.Timestamp)
            .Skip(_nextPagePage * PageSize).Take(PageSize);
    }

    public async ValueTask<bool> MoveNextAsync()
    {
        if (_hasNextPage && (!_currentList.Any() || _currentItemIndex >= _currentList.Count-1))
        {
            var query = GetQuery();
            var result = await query.ToListAsync();
            
            _currentList.AddRange(result);
            _hasNextPage = result.Count > 0;
            _nextPagePage += 1;
        }

        _currentItemIndex += 1;
        return _currentItemIndex < _currentList.Count;
    }

    public LogItem Current => _currentList[_currentItemIndex];
}