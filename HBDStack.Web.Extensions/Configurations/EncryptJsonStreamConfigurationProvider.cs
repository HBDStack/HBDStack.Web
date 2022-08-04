using HBDStack.Framework.Extensions.Encryption;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace HBDStack.Web.Extensions.Configurations;

public sealed class EncryptJsonStreamConfigurationProvider : JsonStreamConfigurationProvider
{
    private readonly EncryptKeyInfo _encryptKey;
    public EncryptJsonStreamConfigurationProvider(JsonStreamConfigurationSource source, EncryptKeyInfo encryptKey) :
        base(source) =>
        _encryptKey = encryptKey;

    public override void Load(Stream stream)
    {
        base.Load(stream);
        Data = _encryptKey.TryDecrypt(Data);
    }
}