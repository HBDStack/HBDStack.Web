// ReSharper disable CheckNamespace

using System.Text.Json.Serialization;

namespace System.Text.Json;

public static class JsonSerializerExtensions
{
    private static readonly JsonSerializerOptions DefaultJsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static TValue Deserialize<TValue>(string json, JsonSerializerOptions options = null) => JsonSerializer.Deserialize<TValue>(json, options ?? DefaultJsonOptions);

    public static string Serialize<TValue>(TValue value, JsonSerializerOptions options = null) => JsonSerializer.Serialize(value, options ?? DefaultJsonOptions);
}