namespace GMenu.Modules.DesktopFiles.Model;

public sealed class DesktopFile : DesktopFileHeader
{
    public string? Categories { get; set; }
    public string? Comment { get => field ?? UnlocalizedComment; set; }
    public string? UnlocalizedComment { get; set; }
    public string? GenericName { get => field ?? UnlocalizedGenericName; set; }
    public string? UnlocalizedGenericName { get; set; }
    public bool DBusActivatable { get; set; }
    public string? Keywords { get => field ?? UnlocalizedKeywords; set; }
    public string? UnlocalizedKeywords { get; set; }
    
}