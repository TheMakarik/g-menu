namespace GMenu.ViewModels.SpecificModels;

public class CategoryTreeViewInfo
{
    public required string Path { get; set; }
    public required string Name { get; set; }
    public required IGrouping<string, DesktopFileHeader> Headers { get; set; }
}