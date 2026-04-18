using ILogger = Serilog.ILogger;

namespace GMenu.Modules.DesktopFiles;

public sealed class DesktopFileIconPathRefiner(ILogger logger) : IDesktopFileIconPathRefiner
{
    private readonly ConcurrentDictionary<string, string> _iconsMappingCache = new();

    private readonly IReadOnlyCollection<string> _actualPathsToRefineIcon = StaticConfiguration.PathsToRefineIcon
        .Where(Directory.Exists)
        .ToArray()
        .AsReadOnly();

    public string? RefinePath(string? path)
    {
        if (path is null)
            return string.Empty;

        if (Path.IsPathRooted(path))
            return path;

        if (_iconsMappingCache.TryGetValue(path, out var result))
            return result;

        var iconPath = _actualPathsToRefineIcon
            .Select(directory => Directory
                .EnumerateFiles(directory, $"{path}.*", SearchOption.AllDirectories))
            .SelectMany(paths => paths
                .Where(pathToIcon => StaticConfiguration.ValidIconExtensions.Contains(Path.GetExtension(pathToIcon))))
            .FirstOrDefault();

        if (iconPath is null)
            return null;

        _iconsMappingCache.TryAdd(path, iconPath);
        return iconPath;
    }

    public void StartBackgroundIconsLoading(IEnumerable<string> iconNamesInDesktopFiles)
    {
        var backgroundWorker = new  BackgroundWorker();
        backgroundWorker.DoWork += async (self, args) =>
        {
            var stopwatch = Stopwatch.StartNew();
            logger.Debug("Start background icon loading...");

            var iconNamesSet = iconNamesInDesktopFiles.ToHashSet();
            if (iconNamesSet.Count == 0)
            {
                logger.Information("No icons to load");
                return;
            }
            

            var tasks = _actualPathsToRefineIcon
                .Where(Directory.Exists)
                .Select(dir => Task.Run(() => ProcessDirectory(dir, iconNamesSet,  StaticConfiguration.ValidIconExtensions)))
                .ToArray();

            var results = await Task.WhenAll(tasks);
            var totalCount = results.Sum();

            logger.Information("Found {count} icons in background, time elapsed: {time}ms",
                totalCount, stopwatch.ElapsedMilliseconds);
            stopwatch.Stop();
        };

        backgroundWorker.RunWorkerAsync(backgroundWorker);
    }

    private int ProcessDirectory(string directory, HashSet<string> iconNamesSet, string[] validExtensions)
    {
        var count = 0;
        var enumerationOptions = new EnumerationOptions
        {
            RecurseSubdirectories = true,
            IgnoreInaccessible = true,
            AttributesToSkip = FileAttributes.System | FileAttributes.Temporary,
        };

        try
        {
            foreach (var file in Directory.EnumerateFiles(directory, "*.*", enumerationOptions))
            {
                var extension = Path.GetExtension(file);
                
                if (!validExtensions.Contains(extension))
                    continue;

                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);

                if (iconNamesSet.Contains(fileNameWithoutExtension) &&
                    _iconsMappingCache.TryAdd(fileNameWithoutExtension, file))
                {
                    Interlocked.Increment(ref count);
                }
            }
        }
        catch (Exception ex)
        {
            logger.Warning(ex, "Error processing directory {Directory}", directory);
        }

        return count;
    }
}