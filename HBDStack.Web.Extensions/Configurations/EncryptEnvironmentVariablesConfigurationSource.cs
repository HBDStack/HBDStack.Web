using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace HBDStack.Web.Extensions.Configurations;

public sealed class EncryptEnvironmentVariablesConfigurationSource:IConfigurationSource
{
    /// <summary>
    /// A prefix used to filter environment variables.
    /// </summary>
    public string Prefix { get; set; }
    public string EncryptKeyName { get; set; } = default!;
    
    /// <summary>
    /// Builds the <see cref="EnvironmentVariablesConfigurationProvider"/> for this source.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
    /// <returns>A <see cref="EnvironmentVariablesConfigurationProvider"/></returns>
    public IConfigurationProvider Build(IConfigurationBuilder builder) 
        => new EncryptEnvironmentVariablesConfigurationProvider(builder.TryGetValue(EncryptKeyName), Prefix);
}