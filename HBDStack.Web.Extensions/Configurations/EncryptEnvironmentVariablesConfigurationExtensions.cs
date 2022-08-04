using HBDStack.Web.Extensions.Configurations;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration.EnvironmentVariables;

public static class EncryptEnvironmentVariablesConfigurationExtensions
{
    public static IConfigurationBuilder AddEncryptEnvironmentVariables(this IConfigurationBuilder configurationBuilder,string encryptKeyName)
    {
        configurationBuilder.Add(new EncryptEnvironmentVariablesConfigurationSource{EncryptKeyName = encryptKeyName});
        return configurationBuilder;
    }
    
    public static IConfigurationBuilder AddEnvironmentVariables(this IConfigurationBuilder configurationBuilder, string prefix,string encryptKeyName)
    {
        configurationBuilder.Add(new EncryptEnvironmentVariablesConfigurationSource { Prefix = prefix,EncryptKeyName = encryptKeyName});
        return configurationBuilder;
    }
}