using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace HBDStack.Web.Extensions.Configurations;

public sealed class EncryptJsonStreamConfigurationSource : JsonStreamConfigurationSource
{
    public string EncryptKeyName { get; set; } = default!;

    public override IConfigurationProvider Build(IConfigurationBuilder builder)
        => new EncryptJsonStreamConfigurationProvider(this, builder.TryGetValue(EncryptKeyName));
}