// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.Configuration;

public static class ConfigExtensions
{
    public static TConfig Bind<TConfig>(this IConfiguration configuration, string key = null) where TConfig : new()
    {
        var config = new TConfig();
        configuration.Bind(key ?? typeof(TConfig).Name, config);
        return config;
    }
}