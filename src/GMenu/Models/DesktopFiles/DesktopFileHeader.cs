namespace GMenu.Models.DesktopFiles;

public sealed class DesktopFileHeader
{
    public required string Path { get; set; }
    public string? IconPath { get; set; }
    public string? Category { get; set; }
    public required string Name { get; set; }

}