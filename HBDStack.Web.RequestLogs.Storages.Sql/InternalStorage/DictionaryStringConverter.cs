using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HBDStack.Web.RequestLogs.Storages.Sql.InternalStorage;

internal class DictionaryStringConverter : ValueConverter<Dictionary<string, string>?, string?>
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = false, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public DictionaryStringConverter() : base(
        v => v == null ? null : JsonSerializer.Serialize(v, Options),
        s => string.IsNullOrEmpty(s) ? new Dictionary<string, string>() : JsonSerializer.Deserialize<Dictionary<string, string>>(s, Options))
    { }
}