namespace GMenu.Modules.DesktopFiles.Model;

public sealed class DesktopFileHeader
{
    public required string Directory { get; set; }
    public string Path => System.IO.Path.Combine(Directory, Name ?? string.Empty);
    public string? IconPath { get; set; }
    public string? Category { get; set; }
    public string? Name { get; set; }
    public bool IsHidden { get; set; } = false;
    public string? Exec { get; set; }
    public bool IsDummy { get; set; } = false;
    public bool IsBroken { get; set; }

}