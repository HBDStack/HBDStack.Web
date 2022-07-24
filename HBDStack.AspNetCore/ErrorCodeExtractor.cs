using System.Diagnostics;
using System.Reflection;
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.Hosting;

public static class ErrorCodeExtractor
{
    /// <summary>
    /// List out all ErrorCodes private nested class for error code
    /// </summary>
    /// <param name="classNames"></param>
    /// <param name="additional">classNames is class contains key-value messages</param>
    /// <param name="directoryPath">The folder to lookup</param>
    /// <param name="searchPattern">The search pattern of assemblies. Ex: TranSwap.*.dll</param>
    public static List<KeyValuePair<string, string>> Extract(string directoryPath, string searchPattern, List<string> classNames, params Type[] additional)
    {
        var folder = Path.GetDirectoryName(directoryPath);
        var files = Directory.GetFiles(folder ?? throw new InvalidOperationException(nameof(directoryPath)), searchPattern, SearchOption.TopDirectoryOnly);
        var errorCodes = new Dictionary<string, string>();
        var types = new List<Type>();

        if (classNames != null)
        {
            foreach (var file in files)
            {
                var assembly = Assembly.LoadFile(file);

                var typePerFile = assembly
                    .GetTypes()
                    .Where(t => t.IsClass &&
                                t.IsNestedPrivate &&
                                classNames.Contains(t.Name))
                    .ToArray();

                if (typePerFile.Any())
                {
                    types.AddRange(typePerFile);
                }
            }
        }

        if (additional.Any())
        {
            var nestedTypes = additional.SelectMany(t => t.GetTypeInfo().DeclaredNestedTypes.Select(n => n.AsType()));
            types.AddRange(nestedTypes);
        }

        foreach (var (key, message) in GetCodesFor(types.ToArray()))
        {
            try
            {
                errorCodes.Add(key, message);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException($"An duplicate ErrorCode found for {key}", e);
            }
        }

        foreach (var (key, value) in errorCodes)
        {
            Trace.WriteLine($"{key}:{value}");
        }

        return errorCodes.OrderBy(a => a.Key).ToList();
    }

    private static IEnumerable<(string key, string message)> GetCodesFor(params Type[] types) 
        => from t in types from prop in t.GetFields() select (prop.Name, prop.GetRawConstantValue() as string);
}