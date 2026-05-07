namespace GMenu.Modules.DesktopFiles;

public sealed class DesktopFileHeaderReader(
    IConfigurationProvider configurationProvider,
    IDesktopFileReader reader,
    ILogger logger) : IDesktopFileHeaderReader
{
    
    public IReadOnlyCollection<DesktopFileHeader> GetAllHeaders(IReadOnlyCollection<string> paths)
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
                var lines = reader.ReadEntry(filePath);

                var desktopFileHeader = new DesktopFileHeader()
                {
                    Path = filePath,
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
                        if(configuration.LocalizeDesktopFiles && IsLocalizedName(key, CultureInfo.CurrentCulture))
                        {
                            desktopFileHeader.Name = value.ToString();
                            wasFoundLocalizedName = true;
                            desktopFileHeader.NameKey = key.ToString();
                        }
                        else 
                            continue;

                    switch (key)
                    {
                        case DesktopFile.NameKey:
                            if (wasFoundLocalizedName)
                                desktopFileHeader.UnlocalizedName = value.ToString();
                            else
                            {
                                desktopFileHeader.Name = value.ToString().Replace("\\n", " ");
                                desktopFileHeader.NameKey = DesktopFile.NameKey;
                            }

                            break;
                        case DesktopFile.IconKey:
                            desktopFileHeader.IconPath = value.ToString();
                            break;
                        case DesktopFile.CategoriesKey:
                            var semicolonIndex = value.IndexOf(';');
                            desktopFileHeader.Category = semicolonIndex == -1
                                ? value.ToString()
                                : value[..semicolonIndex].ToString();
                            break;
                        case DesktopFile.ExecKey:
                            desktopFileHeader.Exec = value.ToString();
                            break;
                        case DesktopFile.NoDisplayKey:
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
        if (!key.StartsWith(DesktopFile.NameKey))
            return false;
    
        if (key.Length <= DesktopFile.NameKey.Length || key[DesktopFile.NameKey.Length] != '[')
            return false;
    
        var closingBracketIndex = key.IndexOf(']');
        if (closingBracketIndex == -1)
            return false;
        
        var languageCode = key.Slice(
            DesktopFile.NameKey.Length + 1,           
            closingBracketIndex - DesktopFile.NameKey.Length - 1 
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
        return desktopFileHeader.Exec is not null;
    }

    private static bool DesktopFileHeaderIsReady(DesktopFileHeader desktopFileHeader, bool wasFoundNoDisplay)
    {
        return desktopFileHeader.Exec is not null
               && desktopFileHeader.IconPath is not null
               && desktopFileHeader.Category is not null
               && wasFoundNoDisplay;
    }
    
}
