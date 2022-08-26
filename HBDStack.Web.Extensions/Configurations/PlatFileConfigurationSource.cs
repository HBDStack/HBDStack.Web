using Microsoft.Extensions.Configuration;

namespace HBDStack.Web.Extensions.Configurations;

public class PlatFileConfigurationSource:IConfigurationSource
{
    public string Directory { get; set; } = default!;
    public string? Prefix { get; set; }
    
    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new PlatFileConfigurationProvider(Directory,Prefix);
}