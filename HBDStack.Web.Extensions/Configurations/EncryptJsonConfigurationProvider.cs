using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace HBDStack.Web.Extensions.Configurations;

public sealed class EncryptJsonConfigurationProvider:JsonConfigurationProvider
{
    private readonly EncryptKeyInfo _encryptKey;
    public EncryptJsonConfigurationProvider(JsonConfigurationSource source, EncryptKeyInfo encryptKey) : base(source) => _encryptKey = encryptKey;

    public override void Load(Stream stream)
    {
        base.Load(stream);
        Data = _encryptKey.TryDecrypt(Data);
    }
}