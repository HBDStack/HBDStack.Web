using System.Collections.Concurrent;
using HBDStack.Framework.Extensions.Encryption;
using HBDStack.Web.Extensions.Configurations;
using Microsoft.Extensions.FileProviders;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration;

public static class EncryptJsonConfigurationExtensions
{
    private static readonly Dictionary<string, EncryptKeyInfo> Cache = new();

    internal static EncryptKeyInfo TryGetValue(this IConfigurationBuilder builder, string key)
    {
        if (Cache.ContainsKey(key)) return Cache[key];
        var v = Environment.GetEnvironmentVariable(key);
        var rs = new EncryptKeyInfo { Name = key, Value = v };
        if (rs.HasValue)
            Cache.Add(key, rs);
        return rs;
    }

    private static void EnsureKey(this EncryptKeyInfo info, IDictionary<string, string> data)
    {
        if (info.HasValue) return;
        if (data.TryGetValue(info.Name, out var v))
        {
            info.Value = v;
            if (!Cache.ContainsKey(info.Name))
                Cache.Add(info.Name, info);
        }
    }

    internal static IDictionary<string, string> TryDecrypt(this EncryptKeyInfo info, IDictionary<string, string> data)
    {
        if (!data.Any()) return data;
        info.EnsureKey(data);
        var results = new Dictionary<string, string>();

        foreach (var key in data)
        {
            if (key.Key == info.Name) continue;
            var v = key.Value;
            
            if (v.IsEncrypted())
                try
                {
                    v = v.DecryptWithAes(info.Value);
                }
                catch (Exception)
                {
                    try
                    {
                        v = v.DecryptWithBase64().DecryptWithAes(info.Value);
                    }
                    catch (Exception)
                    {
                        Console.Error.WriteLine($"Error: Not able to decrypt {key.Key}");
                    }
                }

            results.Add(key.Key, v);
        }

        return results;
    }

    /// <summary>
    /// Add Encrypted Json File
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="path"></param>
    /// <param name="encryptKeyName">The name of encryption key in Environment variable or config file</param>
    /// <returns></returns>
    public static IConfigurationBuilder AddEncryptJsonFile(this IConfigurationBuilder builder, string path,
        string encryptKeyName)
        => AddEncryptJsonFile(builder, provider: null, path: path, optional: false, reloadOnChange: false,
            encryptKeyName);

    public static IConfigurationBuilder AddEncryptJsonFile(this IConfigurationBuilder builder, string path,
        bool optional, string encryptKeyName) =>
        AddEncryptJsonFile(builder, provider: null, path: path, optional: optional, reloadOnChange: false,
            encryptKeyName);

    public static IConfigurationBuilder AddEncryptJsonFile(this IConfigurationBuilder builder, string path,
        bool optional, bool reloadOnChange, string encryptKeyName) =>
        AddEncryptJsonFile(builder, provider: null, path: path, optional: optional, reloadOnChange: reloadOnChange,
            encryptKeyName);


    public static IConfigurationBuilder AddEncryptJsonFile(this IConfigurationBuilder builder, IFileProvider? provider,
        string path, bool optional, bool reloadOnChange, string encryptKeyName)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Invalid File Path", nameof(path));
        }

        return builder.AddEncryptJsonFile(s =>
        {
            s.EncryptKeyName = encryptKeyName;
            s.FileProvider = provider;
            s.Path = path;
            s.Optional = optional;
            s.ReloadOnChange = reloadOnChange;
            s.ResolveFileProvider();
        });
    }


    internal static IConfigurationBuilder AddEncryptJsonFile(this IConfigurationBuilder builder,
        Action<EncryptJsonConfigurationSource> configureSource)
        => builder.Add(configureSource);


    public static IConfigurationBuilder AddEncryptJsonStream(this IConfigurationBuilder builder, Stream stream,
        string encryptKeyName)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        return builder.Add<EncryptJsonStreamConfigurationSource>(s =>
        {
            s.EncryptKeyName = encryptKeyName;
            s.Stream = stream;
        });
    }
}