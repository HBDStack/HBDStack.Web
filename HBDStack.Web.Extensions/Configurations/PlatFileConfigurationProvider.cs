using Microsoft.Extensions.Configuration;

namespace HBDStack.Web.Extensions.Configurations;

public class PlatFileConfigurationProvider:ConfigurationProvider
{
    private readonly string _directory;
    private readonly string? _prefix;

    public PlatFileConfigurationProvider(string directory, string? prefix)
    {
        _directory = directory ?? throw new ArgumentNullException(nameof(directory));
        _prefix = prefix;
    }
    public override void Load()
    {
        base.Load();
        
        var directory = new DirectoryInfo(Path.GetFullPath(_directory));
        if (!directory.Exists)
        {
            Console.Error.WriteLine($"Configuration Directory {_directory} is not existed");
            return;
        }

        var files = directory.GetFiles("*", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            var ext = file.Extension;
            var nameWithoutExtension =string.IsNullOrEmpty(ext)?file.Name: file.Name.Replace(ext, string.Empty);
            nameWithoutExtension = nameWithoutExtension.Replace("__", ":");
            var name = string.IsNullOrEmpty(_prefix) ? nameWithoutExtension : $"{_prefix}{nameWithoutExtension}";
            var value = File.ReadAllText(file.FullName);
            Data.Add(name,value);
        }
    }
}