namespace GMenu.Modules.DesktopFiles.Model;

public class DesktopFileHeader
{
    public string Directory => System.IO.Path.GetDirectoryName(Path) ?? string.Empty;
    public required string Path { get; set; }
    public string? IconPath { get; set; }
    public string? Category { get; set; }
    public string Name { get => field ?? string.Empty; set; }
    public bool IsHidden { get; set; } = false;
    public string? Exec { get; set; } 
    public bool IsBroken { get; set; }
    public string? NameKey { get; set; }
    public string? UnlocalizedName { get => field ?? Name; set; }
    

}