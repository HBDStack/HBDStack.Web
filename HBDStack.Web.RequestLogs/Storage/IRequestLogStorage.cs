namespace HBDStack.Web.RequestLogs.Storage;

public interface IRequestLogStorage
{
    Task LogAsync(LogItem item,CancellationToken cancellationToken =default);
    IQueryable<LogItem> GetQuery();
}