namespace GMenu.Modules.DesktopFiles;

public sealed class DesktopFileIconPathRefiner(ILogger logger, GMenuOptions options) : IDesktopFileIconPathRefiner
{
    private readonly ConcurrentDictionary<string, string> _iconsMappingCache = new();
    
    public string? RefinePath(string? path, IReadOnlyCollection<string> pathsToRefineIcon)
    {
        if (path is null)
            return string.Empty;

        if (Path.IsPathRooted(path))
            return path;

        if (_iconsMappingCache.TryGetValue(path, out var result))
            return result;

        var iconPath = pathsToRefineIcon
            .Select(directory => Directory
                .EnumerateFiles(directory, $"{path}.*", SearchOption.AllDirectories))
            .SelectMany(paths => paths
                .Where(pathToIcon => options.Configuration.ValidIconExtensions.Contains(Path.GetExtension(pathToIcon))))
            .FirstOrDefault();

        if (iconPath is null)
            return null;

        _iconsMappingCache.TryAdd(path, iconPath);
        return iconPath;
    }

    public void StartBackgroundIconsLoading(IEnumerable<string> iconNamesInDesktopFiles, IReadOnlyCollection<string> pathsToRefineIcon)
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
            

            var tasks = pathsToRefineIcon
                .Select(directory => Task.Run(() => ProcessDirectory(directory, iconNamesSet, options.Configuration.ValidIconExtensions)))
                .ToArray();

            var results = await Task.WhenAll(tasks);
            var totalCount = results.Sum();

            logger.Information("Found {count} icons in background, time elapsed: {time}ms",
                totalCount, stopwatch.ElapsedMilliseconds);
            stopwatch.Stop();
        };

        backgroundWorker.RunWorkerAsync(backgroundWorker);
    }

    private int ProcessDirectory(string directory, HashSet<string> iconNamesSet, IReadOnlyCollection<string> validExtensions)
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