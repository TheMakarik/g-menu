namespace GMenu.Modules.DesktopFiles;

public sealed class DesktopFileHeaderReader(
    ILogger logger) : IDesktopFileHeaderReader
{
    private const string DesktopEntryHeader = "[Desktop Entry]";
    private const string ExecKey = "Exec";
    private const string NameKey = "Name";
    private const string IconKey = "Icon";
    private const string CategoriesKey = "Categories";
    private const string NoDisplayKey = "NoDisplay";


    public IReadOnlyCollection<DesktopFileHeader> GetAllHeaders(string[] paths)
    {
        logger.Information("Start searching desktop files");
   
        var filesToHandle = paths
            .SelectMany(directory => Directory
                .EnumerateFiles(directory, "*.desktop", new EnumerationOptions() { RecurseSubdirectories = true }));

        var stopwatch = Stopwatch.StartNew();
        
     
        var result = filesToHandle
            .AsParallel()
            .WithDegreeOfParallelism(Environment.ProcessorCount)
            .Select<string, DesktopFileHeader?>(filePath =>
            {
                GetDesktopFileLineEnumeration(out var lines, filePath);

                var desktopFileHeader = new DesktopFileHeader()
                {
                    Directory = Path.GetDirectoryName(filePath)!,
                    Name = Path.GetFileNameWithoutExtension(filePath),
                };
                var wasFoundNoDisplay = false;

                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;


                    var equalsIndex = line.IndexOf('=');

                    if (equalsIndex == -1)
                        continue;


                    var key = line.AsSpan(0, equalsIndex);
                    var value = line.AsSpan(equalsIndex + 1);

                    if (key[^1] == ']')
                        continue;

                    switch (key)
                    {
                        case NameKey:
                            desktopFileHeader.Name = value.ToString().Replace("\\n", " ");
                            break;
                        case IconKey:
                            desktopFileHeader.IconPath = value.ToString();
                            break;
                        case CategoriesKey:
                            var semicolonIndex = value.IndexOf(';');
                            desktopFileHeader.Category = semicolonIndex == -1
                                ? value.ToString()
                                : value[..semicolonIndex].ToString();
                            break;
                        case ExecKey:
                            desktopFileHeader.Exec = value.ToString();
                            break;
                        case NoDisplayKey:
                            if (bool.TryParse(value, out var isHidden))
                            {
                                desktopFileHeader.IsHidden = isHidden;
                                wasFoundNoDisplay = true;
                            }
                            else
                                logger.Error("Desktop file: {path} has corrupted NoDisplay value", filePath);
                            break;
                    }

                    if (DesktopFileHeaderIsReady(desktopFileHeader, wasFoundNoDisplay))
                        break;
                }

                if (!IsDesktopFileCorrect(desktopFileHeader))
                    desktopFileHeader.IsBroken = true;

                return desktopFileHeader;
            })
            .Cast<DesktopFileHeader>()
            .ToList()
            .AsReadOnly();

        stopwatch.Stop();
        logger.Information("Find desktop files: {count} for {time} ms", result.Count,  stopwatch.ElapsedMilliseconds);
        return result;
    }

    private static bool IsDesktopFileCorrect(DesktopFileHeader desktopFileHeader)
    {
        return desktopFileHeader.Name is not null 
               && desktopFileHeader.Exec is not null;
    }

    private static bool DesktopFileHeaderIsReady(DesktopFileHeader desktopFileHeader, bool wasFoundNoDisplay)
    {
        return desktopFileHeader.Exec is not null
               && desktopFileHeader.IconPath is not null
               && desktopFileHeader.Name is not null
               && desktopFileHeader.Category is not null
               && wasFoundNoDisplay;
    }

    private static void GetDesktopFileLineEnumeration(out IEnumerable<string> lines, string filePath)
    {
        lines = File.ReadLines(filePath)
            .SkipWhile(static line => line != DesktopEntryHeader)
            .Where(static line => !string.IsNullOrEmpty(line))
            .Skip(1) // skip entry header
            .TakeWhile(static line => line[0] is not '[');
    }
}
