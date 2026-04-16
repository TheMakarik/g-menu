using ILogger = Serilog.ILogger;

namespace GMenu.Modules.DesktopFiles;

public sealed class DesktopFileIconPathRefiner(ILogger logger) : IDesktopFileIconPathRefiner
{
    private FrozenDictionary<string, string>? _iconsMapping = null;
    
    public string? RefinePath(string? path)
    {
        if(path is null)
            return string.Empty;
        
        if (_iconsMapping is null)
        {
            var values = StaticConfiguration.PathsToRefineIcon
                .Where(Directory.Exists)
                .SelectMany(dir => Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories))
                .ToHashSet();

            var dictionary = new Dictionary<string, string>(capacity: values.Count);
            foreach (var value in values)
            {
                var key = Path.GetFileNameWithoutExtension(value);
                dictionary.TryAdd(key, value);
            }

            _iconsMapping = dictionary.ToFrozenDictionary();
        }
        
        return Path.IsPathRooted(path) ? path : _iconsMapping.GetValueOrDefault(path, path);
    }
}