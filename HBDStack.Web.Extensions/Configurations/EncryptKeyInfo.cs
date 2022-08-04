using HBDStack.Framework.Extensions.Encryption;

namespace HBDStack.Web.Extensions.Configurations;

public sealed class EncryptKeyInfo
{
    public string Name { get; init; }
    public string? Value { get; set; }
    public bool HasValue => !string.IsNullOrWhiteSpace(Value) && Value.IsEncrypted();
}