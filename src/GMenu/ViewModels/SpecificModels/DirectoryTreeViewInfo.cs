namespace GMenu.ViewModels.SpecificModels;

public class DirectoryTreeViewInfo
{
    public required string Path { get; set; }
    public required IGrouping<string, DesktopFileHeader> Headers { get; set; }
}