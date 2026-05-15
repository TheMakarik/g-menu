namespace GMenu.Modules.DesktopFiles.Model;

public sealed class DesktopFile : DesktopFileHeader
{
    [DocumentationKey($"About{nameof(Categories)}")]
    public string? Categories { get; set; }

    public string? Comment
    {
        get => field ?? UnlocalizedComment;
        set;
    }

    public string? UnlocalizedComment { get; set; }

    [DocumentationKey($"About{nameof(GenericName)}")]
    public string? GenericName
    {
        get => field ?? UnlocalizedGenericName;
        set;
    }
    
    [DocumentationKey($"About{nameof(GenericName)}")]
    public string? UnlocalizedGenericName { get; set; }
    
    [DocumentationKey($"About{nameof(DBusActivatable)}")]
    public bool DBusActivatable { get; set; }

    public string? Keywords
    {
        get => field ?? UnlocalizedKeywords;
        set;
    }

    public string? UnlocalizedKeywords { get; set; }
    
    [DocumentationKey($"About{nameof(StartupNotify)}")]
    public bool StartupNotify { get; set; }
    
    [DocumentationKey($"About{nameof(StartupWMClass)}")]
    public string? StartupWMClass { get; set; }
    public bool Terminal { get; set; }
    public string? MimeType { get; set; }
    
    [DocumentationKey($"About{nameof(SingleMainWindow)}")]
    public bool SingleMainWindow { get; set; }
    
    [DocumentationKey($"About{nameof(Implements)}")]
    public string? Implements { get; set; }
    
    [DocumentationKey($"About{nameof(TryExec)}")]
    public string? TryExec { get; set; }
    
    [DocumentationKey($"About{nameof(XGnomeAutoRestart)}")]
    public bool XGnomeAutoRestart { get; set; }
    public string? XGnomeUsesNotifications { get; set; }
    public string? OnlyShowIn { get; set; }
    public string? NotShowIn { get; set; }
    public bool Hidden { get; set; }
    public string? Version { get; set; }
}