using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace HBDStack.Web.Extensions.Configurations;

public sealed class EncryptJsonConfigurationSource : JsonConfigurationSource
{
    public string EncryptKeyName { get; set; } = default!;

    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        EnsureDefaults(builder);
        return new EncryptJsonConfigurationProvider(this,builder.TryGetValue(EncryptKeyName));
    }
}