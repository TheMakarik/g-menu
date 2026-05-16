namespace GMenu.Modules.DesktopFiles.Model;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
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
    
    public string? CommentKey { get; set; }

    [DocumentationKey($"About{nameof(GenericName)}")]
    public string? GenericName
    {
        get => field ?? UnlocalizedGenericName;
        set;
    }
    
    [DocumentationKey($"About{nameof(GenericName)}")]
    public string? UnlocalizedGenericName { get; set; }
    
    public string? GenericNameKey { get; set; }
    
    [DocumentationKey($"About{nameof(DBusActivatable)}")]
    public bool? DBusActivatable { get; set; }

    public string? Keywords
    {
        get => field ?? UnlocalizedKeywords;
        set;
    }

    public string? UnlocalizedKeywords { get; set; }
    
    public string? KeywordsKey { get; set; }
    
    [DocumentationKey($"About{nameof(StartupNotify)}")]
    public bool? StartupNotify { get; set; }
    
    [DocumentationKey($"About{nameof(StartupWmClass)}")]
    public string? StartupWmClass { get; set; }
    public bool? Terminal { get; set; }
    
    [DocumentationKey($"About{nameof(MimeTypes)}")]
    public string? MimeTypes { get; set; }
    
    [DocumentationKey($"About{nameof(SingleMainWindow)}")]
    public bool? SingleMainWindow { get; set; }
    
    [DocumentationKey($"About{nameof(Implements)}")]
    public string? Implements { get; set; }
    
    [DocumentationKey($"About{nameof(TryExec)}")]
    public string? TryExec { get; set; }
    
    [DocumentationKey($"About{nameof(XGnomeAutoRestart)}")]
    public bool? XGnomeAutoRestart { get; set; }
    
    [DocumentationKey($"About{nameof(XGnomeUsesNotifications)}")]
    public string? XGnomeUsesNotifications { get; set; }
    
    [DocumentationKey($"About{nameof(OnlyShowIn)}")]
    public string? OnlyShowIn { get; set; }
    
    [DocumentationKey($"About{nameof(NotShowIn)}")]
    public string? NotShowIn { get; set; }
    
    [DocumentationKey($"About{nameof(Hidden)}")]
    public bool? Hidden { get; set; }
    
    [DocumentationKey($"About{nameof(Version)}")]
    public string? Version { get; set; }
}