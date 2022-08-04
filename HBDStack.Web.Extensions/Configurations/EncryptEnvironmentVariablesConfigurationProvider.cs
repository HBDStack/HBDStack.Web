using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace HBDStack.Web.Extensions.Configurations;

public sealed class EncryptEnvironmentVariablesConfigurationProvider : EnvironmentVariablesConfigurationProvider
{
    private readonly EncryptKeyInfo _encryptKey;

    public EncryptEnvironmentVariablesConfigurationProvider(EncryptKeyInfo encryptKey, string? prefix = null) : base(
        prefix ?? string.Empty) =>
        _encryptKey = encryptKey;

    public override void Load()
    {
        base.Load();
        Data = _encryptKey.TryDecrypt(Data);
    }
}