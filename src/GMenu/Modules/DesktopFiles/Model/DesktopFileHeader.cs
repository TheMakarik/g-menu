namespace GMenu.Modules.DesktopFiles.Model;

public sealed class DesktopFileHeader
{
    public string Directory => System.IO.Path.GetDirectoryName(Path) ?? string.Empty;
    public required string Path { get; set; }
    public string? IconPath { get; set; }
    public string? Category { get; set; }
    public required string Name { get; set; }
    public bool IsHidden { get; set; } = false;
    public string? Exec { get; set; }
    public bool IsDummy { get; set; } = false;
    public bool IsBroken { get; set; }
    public string? NameKey { get; set; }
    public string? UnlocalizedName { get => field ?? Name; set; }
    

}