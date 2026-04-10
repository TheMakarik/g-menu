using System.Diagnostics.CodeAnalysis;

namespace GMenu.Models.DesktopFiles;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public sealed class DesktopFileHeader
{
    public required string Directory { get; set; }
    public required string Path { get; set; }
    public string? IconPath { get; set; }
    public string? Category { get; set; }
    public required string Name { get; set; }

}