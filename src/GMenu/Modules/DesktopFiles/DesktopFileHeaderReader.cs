using GMenu.Modules.Configuration.Interfaces;
using GMenu.Modules.DesktopFiles.Interfaces;
using GMenu.Modules.DesktopFiles.Model;
using GMenu.Models.DesktopFiles;
using Microsoft.Extensions.Logging;
using System.Linq.Async;

namespace GMenu.Modules.DesktopFiles;

public sealed class DesktopFileHeaderReader(
    ILogger logger,
    IConfiguration configuration,
    IRootRequirer rootRequirer) : IDesktopFileHeaderReader
{
    private const string DesktopEntryHeader = "[Desktop Entry]";
    private const string ExecKey = "Exec";
    private const string NameKey = "Name";
    private const string IconKey = "Icon";
    private const string CategoriesKey = "Categories";
    private const string NoDisplayKey = "NoDisplay";
    private const string TrueValue = "true";

    public async Task<IReadOnlyCollection<DesktopFileHeader>> GetAllHeadersAsync(
        CancellationTokenSource cancellationTokenSource)
    {
        try
        {
            var config = configuration.GetObservable();
            var searchDirectories = config.SearchDesktopFilesDirectories;
            var unexistingCategories = config.UnexistingCategories;

            var allHeaders = await searchDirectories
                .ToAsyncEnumerable()
                .SelectMany(directory => GetDesktopFilesRecursively(directory.Path).ToAsyncEnumerable())
                .SelectAwait(async filePath => await ParseDesktopFileAsync(filePath, cancellationTokenSource.Token))
                .Where(header => header != null)
                .Select(header => header!)
                .Concat(unexistingCategories.ToAsyncEnumerable().Select(category => new DesktopFileHeader
                {
                    Directory = string.Empty,
                    Category = category,
                    IsDummy = true,
                    IsHidden = false
                }))
                .OrderBy(header => header.Directory)
                .ThenBy(header => header.Category ?? string.Empty)
                .ToListAsync(cancellationTokenSource.Token);

            return allHeaders;
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Unauthorized access while reading desktop files");

            try
            {
                await rootRequirer.RequestRootAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                throw new UnauthorizedAccessException("Access denied after cancellation", ex);
            }

            throw;
        }
    }

    private IEnumerable<string> GetDesktopFilesRecursively(string directory)
    {
        if (!Directory.Exists(directory))
        {
            logger.LogWarning("Directory not found: {Directory}", directory);
            yield break;
        }

        foreach (var file in Directory.GetFiles(directory, "*.desktop"))
        {
            yield return file;
        }

        foreach (var subDir in Directory.GetDirectories(directory))
        {
            foreach (var file in GetDesktopFilesRecursively(subDir))
            {
                yield return file;
            }
        }
    }

    private (string Key, string Value) ParseDesktopLine(string line)
    {
        var equalIndex = line.IndexOf('=');
        if (equalIndex <= 0)
        {
            return (string.Empty, string.Empty);
        }

        var key = line[..equalIndex].Trim();
        var value = line[(equalIndex + 1)..].Trim();
        
        return (key, value);
    }

    private async Task<DesktopFileHeader?> ParseDesktopFileAsync(
        string filePath,
        CancellationToken cancellationToken)
    {
        try
        {
            var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);
            var isInsideDesktopEntry = false;
            var header = new DesktopFileHeader
            {
                Directory = Path.GetDirectoryName(filePath) ?? string.Empty,
                IsHidden = false
            };
            var hasExec = false;
            var hasName = false;

            foreach (var rawLine in lines)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var line = rawLine.Trim();

                if (line.StartsWith('['))
                {
                    if (isInsideDesktopEntry)
                    {
                        break;
                    }

                    if (line.Equals(DesktopEntryHeader, StringComparison.OrdinalIgnoreCase))
                    {
                        isInsideDesktopEntry = true;
                    }
                    continue;
                }
                
                if (!isInsideDesktopEntry || string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var (key, value) = ParseDesktopLine(line);

                if (key == ExecKey)
                {
                    hasExec = true;
                }
                else if (key == NameKey)
                {
                    header.Name = value;
                    hasName = true;
                }
                else if (key == IconKey)
                {
                    header.IconPath = value;
                }
                else if (key == CategoriesKey)
                {
                    header.Category = value.Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                }
                else if (key == NoDisplayKey)
                {
                    header.IsHidden = value.Equals(TrueValue, StringComparison.OrdinalIgnoreCase);
                }

                if (hasExec && hasName && header.Category != null)
                {
                    break;
                }
            }

            if (!isInsideDesktopEntry)
            {
                logger.LogWarning("Missing [Desktop Entry] in: {FilePath}", filePath);
                return null;
            }

            if (!hasName)
            {
                logger.LogWarning("Missing Name in: {FilePath}", filePath);
                return null;
            }

            if (!hasExec)
            {
                logger.LogWarning("Missing Exec in: {FilePath}", filePath);
                return null;
            }

            return header;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error parsing: {FilePath}", filePath);
            return null;
        }
    }
}
