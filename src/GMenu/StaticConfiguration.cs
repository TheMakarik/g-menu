namespace GMenu;

public static class StaticConfiguration
{
    public static readonly string ConfigurationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "g-menu");
    public const string DefaultAccentColor = "blue";
    public const string SerilogOutputTemplate = "[{Level:u3}] [{Timestamp:yyyy-MM-dd HH:mm}] [{ThreadId}] {Message:lj}{NewLine}{Exception}";
    public const string DefaultLocalizationPathPrefix = "/Resources/";
    public static readonly string LogsPath = Path.Combine(ConfigurationDirectory, "/logs/g-menu-log__");
    public const string UncategorizedCategory = "Uncategorized";
    public static readonly ConsoleTheme SerilogConsoleTheme = AnsiConsoleTheme.Code;
    public static RollingInterval SerilogRollingInterval = RollingInterval.Day;
    public const string AvaloniaResourcePrefix = "avares://GMenu";
    public static readonly string ConfigurationPath = Path.Combine(ConfigurationDirectory, "g-menu.json");
    public const  JsonKnownNamingPolicy DefaultJsonNamingPolicy = JsonKnownNamingPolicy.CamelCase;
    public const string CannotFoundKeyInLocalizationValue = "????";
    public static readonly string[] ValidIconExtensions = [".svg", ".png", ".jpg", ".jpeg", ".xpm"];
    
    public static readonly User DefaultUser = new() { Language = new CultureInfo("ru-RU") };
    public static readonly DesktopFileDirectory[] DefaultDesktopFileDirectories = 
    [
        new DesktopFileDirectory("/usr/share/applications", "Global"),
        new DesktopFileDirectory($"/home/{Environment.UserName}/.local/share/applications/"),
        new DesktopFileDirectory($"/home/{Environment.UserName}/.local/share/applications/wine"),
        new DesktopFileDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}")
    ];

    public static readonly string[] PathsToRefineIcon =
    [
        $"/home/{Environment.UserName}/.local/share/icons/",
        $"/home/{Environment.UserName}/.icons/",
        "/usr/share/icons/",
        "/usr/share/pixmaps/",
        "/usr/local/share/icons/"
    ];

    public static readonly FrozenDictionary<string, MaterialIconKind> MaterialIconsForCategory =
        new Dictionary<string, MaterialIconKind>
        {
            ["AudioVideo"] = MaterialIconKind.Multimedia,
            ["Audio"] = MaterialIconKind.MusicNote,
            ["Video"] = MaterialIconKind.Video,
            ["Development"] = MaterialIconKind.CodeBraces,
            ["Education"] = MaterialIconKind.School,
            ["Game"] = MaterialIconKind.GamepadVariant,
            ["Graphics"] = MaterialIconKind.Image,
            ["Network"] = MaterialIconKind.Web,
            ["Office"] = MaterialIconKind.OfficeBuilding,
            ["Science"] = MaterialIconKind.Flask,
            ["Settings"] = MaterialIconKind.Cog,
            ["System"] = MaterialIconKind.Monitor,
            ["Utility"] = MaterialIconKind.Tools,
            ["Building"] = MaterialIconKind.Domain,
            ["Core"] = MaterialIconKind.Apps,
            ["Debugger"] = MaterialIconKind.Bug,
            ["Dialup"] = MaterialIconKind.Phone,
            ["Dictionary"] = MaterialIconKind.BookOpenPageVariant,
            ["Email"] = MaterialIconKind.Email,
            ["FileManager"] = MaterialIconKind.Folder,
            ["GNOME"] = MaterialIconKind.Gnome,
            ["GTK"] = MaterialIconKind.LanguageC,
            ["IDE"] = MaterialIconKind.ApplicationBrackets,
            ["InstantMessaging"] = MaterialIconKind.Message,
            ["IRCClient"] = MaterialIconKind.Pound,
            ["Monitor"] = MaterialIconKind.ChartLine,
            ["News"] = MaterialIconKind.Newspaper,
            ["PackageManager"] = MaterialIconKind.PackageVariant,
            ["Presentation"] = MaterialIconKind.Presentation,
            ["Printing"] = MaterialIconKind.Printer,
            ["ProjectManagement"] = MaterialIconKind.ClipboardList,
            ["Publishing"] = MaterialIconKind.Book,
            ["Qt"] = MaterialIconKind.LanguageCpp,
            ["RasterGraphics"] = MaterialIconKind.Brush,
            ["RemoteAccess"] = MaterialIconKind.RemoteDesktop,
            ["RevisionControl"] = MaterialIconKind.SourceBranch,
            ["Security"] = MaterialIconKind.Shield,
            ["Spreadsheet"] = MaterialIconKind.FileExcel,
            ["Telephony"] = MaterialIconKind.PhoneInTalk,
            ["TerminalEmulator"] = MaterialIconKind.Console,
            ["TextEditor"] = MaterialIconKind.TextBox,
            ["VectorGraphics"] = MaterialIconKind.VectorSquare,
            ["Viewer"] = MaterialIconKind.Eye,
            ["WebBrowser"] = MaterialIconKind.Web,
            ["WebDevelopment"] = MaterialIconKind.LanguageHtml5,
            ["WordProcessor"] = MaterialIconKind.FileDocument,
            ["X-GNOME-NetworkSettings"] = MaterialIconKind.Network,
            ["X-GNOME-PersonalSettings"] = MaterialIconKind.Account,
            ["KDE"] = MaterialIconKind.Linux,
            ["LXDE"] = MaterialIconKind.Linux,
            ["LXQt"] = MaterialIconKind.Linux,
            ["MATE"] = MaterialIconKind.Linux,
            ["ROX"] = MaterialIconKind.Linux,
            ["XFCE"] = MaterialIconKind.Linux,
            ["TDE"] = MaterialIconKind.Linux,
            ["Unity"] = MaterialIconKind.Ubuntu,
            [StaticConfiguration.UncategorizedCategory] = MaterialIconKind.QuestionMarkCircleOutline
        }.ToFrozenDictionary();

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
    
    public static readonly ObservableConfiguration DefaultJsonConfiguration = new ObservableConfiguration()
    {
        User = DefaultUser,
        SearchDesktopFilesDirectories = new ObservableCollection<DesktopFileDirectory>(DefaultDesktopFileDirectories),
        UnexistingCategories = []
    };
}