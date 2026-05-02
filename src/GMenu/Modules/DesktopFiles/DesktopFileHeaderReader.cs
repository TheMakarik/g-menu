namespace GMenu.Modules.DesktopFiles;

public sealed class DesktopFileHeaderReader(
    IConfigurationProvider configurationProvider,
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
        var configuration = configurationProvider.CurrentObservable;
   
        var filesToHandle = paths
            .SelectMany(static directory => Directory
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
                var wasFoundLocalizedName = false;

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
                        if(configuration.LocalizeDesktopFileNames && IsLocalizedName(key, configuration.Language))
                        {
                            desktopFileHeader.Name = value.ToString();
                            wasFoundLocalizedName = true;
                            desktopFileHeader.NameKey = key.ToString();
                        }
                        else 
                            continue;

                    switch (key)
                    {
                        case NameKey when !wasFoundLocalizedName:
                            desktopFileHeader.Name = value.ToString().Replace("\\n", " ");
                            desktopFileHeader.NameKey = NameKey;
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

    private static bool IsLocalizedName(
        ReadOnlySpan<char> key,      
        CultureInfo configurationLanguage)
    {
        if (!key.StartsWith(NameKey))
            return false;
    
        if (key.Length <= NameKey.Length || key[NameKey.Length] != '[')
            return false;
    
        var closingBracketIndex = key.IndexOf(']');
        if (closingBracketIndex == -1)
            return false;
        
        var languageCode = key.Slice(
            NameKey.Length + 1,           
            closingBracketIndex - NameKey.Length - 1 
        );
    
        if (languageCode.IsEmpty)
            return false;
        
        var userLanguage = configurationLanguage.Name; 
        var normalizedUserLang = userLanguage.Replace('-', '_');
        var desktopLang = languageCode.ToString();
        
        return desktopLang.Equals(normalizedUserLang, StringComparison.OrdinalIgnoreCase) ||
               desktopLang.Equals(configurationLanguage.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase);
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
