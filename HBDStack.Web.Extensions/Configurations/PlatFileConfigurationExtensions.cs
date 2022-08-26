using HBDStack.Web.Extensions.Configurations;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration;

public static class PlatFileConfigurationExtensions
{
    public static IConfigurationBuilder AddPlatFiles(this IConfigurationBuilder builder,
        string directory,string?prefix = null)
        => builder.AddPlatFiles(c =>
        {
            c.Directory = directory;
            c.Prefix = prefix;
        });
    
    internal static IConfigurationBuilder AddPlatFiles(this IConfigurationBuilder builder,
        Action<PlatFileConfigurationSource> configureSource)
        => builder.Add(configureSource);
}