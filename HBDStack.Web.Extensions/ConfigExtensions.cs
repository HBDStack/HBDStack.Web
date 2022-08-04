using System.Collections.Concurrent;
// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.Configuration;

public static class ConfigExtensions
{
    private static readonly ConcurrentDictionary<string, dynamic> Cache = new();

    /// <summary>
    /// Bind object from configuration with internal cache.
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="key"></param>
    /// <typeparam name="TConfig"></typeparam>
    /// <returns></returns>
    public static TConfig Bind<TConfig>(this IConfiguration configuration, string key) where TConfig : new() =>
        Cache.GetOrAdd(key, (k) =>
        {
            var config = new TConfig();
            configuration.Bind(key, config);
            return config;
        });
}