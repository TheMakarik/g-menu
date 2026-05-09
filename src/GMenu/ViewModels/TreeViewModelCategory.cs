namespace GMenu.ViewModels;

public class TreeViewModelCategory : TreeViewModelBase
{
    private readonly CategoryTreeViewInfo _categoryInfo;
    public string CategoryName => _categoryInfo.Name;

    public TreeViewModelCategory(CategoryTreeViewInfo categoryInfo, IDesktopFilesRunner runner, IDesktopFileIconPathRefiner iconPathRefiner, ILogger logger, ILocalizationProvider localizationProvider) : base(logger, localizationProvider)
    {
        _categoryInfo = categoryInfo;
        var children = categoryInfo.Headers.Select(header => new TreeViewModelDesktopFile(
            header,
            runner,
            iconPathRefiner,
            logger, 
            localizationProvider){Parent = this});
        Children.AddRange(children);
        
    }

    public override void SendSelectionChangeMessage() { }
}