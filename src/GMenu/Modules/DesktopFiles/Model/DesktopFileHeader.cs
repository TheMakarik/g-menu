namespace GMenu.Modules.DesktopFiles.Model;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public class DesktopFileHeader
{
    public string Directory => System.IO.Path.GetDirectoryName(Path) ?? string.Empty;
    
    [DocumentationKey($"About{nameof(WorkingDirectory)}")]
    public string? WorkingDirectory { get; set; }
    public required string Path { get; set; }
    public string? IconPath { get; set; }
    public string? Category { get; set; }
    public string Name { get => field ?? string.Empty; set; }
    public bool NoDisplay { get; set; } = false;
    public string? Exec { get; set; } 
    public bool IsBroken { get; set; }
    public string? NameKey { get; set; }
    public string? UnlocalizedName { get => field ?? Name; set; }
    

}