namespace GMenu.Models.DesktopFiles;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public sealed class DesktopFileHeader
{
    public required string Directory { get; set; }
    public string Path => System.IO.Path.Combine(Directory, Name ?? string.Empty);
    public string? IconPath { get; set; }
    public string? Category { get; set; }
    public string? Name { get; set; }
    public bool IsDummy { get; set; } = false;

}