namespace GMenu;

public static class StaticConfiguration
{
    private static readonly string ConfigurationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "g-menu");
    public const string DefaultAccentColor = "blue";
    public const string SerilogOuputTemplate = "[{Level:u3}] [{Timestamp:yyyy-MM-dd HH:mm}] [{ThreadId}] {Message:lj}{NewLine}{Exception}";
    public const string DefaultLocalizationPathPrefix = "/Resources/";
    public const string AvaloniaResourcePrefix = "avares://GMenu";
    public static readonly string ConfigurationPath = Path.Combine(ConfigurationDirectory, "g-menu.json");
    public const  JsonKnownNamingPolicy DefaultJsonNamingPolicy = JsonKnownNamingPolicy.CamelCase;
    
    public static readonly User DefaultUser = new() { Language = new CultureInfo("ru-RU") };
    public static readonly DesktopFileDirectory[] DefaultDesktopFileDirectories = 
    [
        new DesktopFileDirectory("/usr/share/applications", LocalizationKey.Global)
    ];
    
    public static readonly FrozenDictionary<string, string> AccentColorMap = new Dictionary<string, string>()
    {
        ["blue"]   = "#3584E4",
        ["teal"]   = "#2190A4",
        ["green"]  = "#3A9446",
        ["yellow"] = "#C88800",
        ["orange"] = "#ED7B2B",
        ["red"]    = "#E62D42",
        ["pink"]   = "#D56199",
        ["purple"] = "#9141AC",
        ["slate"]  = "#6F8396"
    }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    
    public static readonly DefaultJSONConfiguration DefaultJsonConfiguration = new DefaultJSONConfiguration(
        DefaultUser,
        DefaultDesktopFileDirectories,
        []
    );
}