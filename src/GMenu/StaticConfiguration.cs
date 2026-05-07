namespace GMenu;

public static class StaticConfiguration
{
    public const  JsonKnownNamingPolicy DefaultJsonNamingPolicy = JsonKnownNamingPolicy.CamelCase;
    public const string ConfigurationPath = "appsettings.json";
    public const string Uncategorized = "Uncategorized";
    

    public static readonly IReadOnlyCollection<string> FreedesktopDataDirectories = Environment.GetEnvironmentVariable("XDG_DATA_DIRS")!.Split(':');
    
    public static readonly IReadOnlyCollection<string> PathsToRefineIcon = FreedesktopDataDirectories
        .Select(x => Path.Combine(x, "icons"))
        .Where(Directory.Exists)
        .ToArray();
    
    public static readonly IReadOnlyCollection<string> PathToDesktopFiles = FreedesktopDataDirectories
        .Select(x => Path.Combine(x, "applications"))
        .Where(Directory.Exists)
        .ToArray();
}