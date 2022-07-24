namespace HBDStack.Web.RequestLogs.Storages.Sql.InternalStorage;

public class SqlRequestLogOptions
{
    public SqlRequestLogOptions(string connectionString) => ConnectionString = connectionString;

    public string ConnectionString { get;  }
    public string TableName { get; set; } = "RequestLogs";
    public string Schema { get; set; } = "logs";
}