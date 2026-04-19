using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public class TreeViewModelCategory : TreeViewModelBase
{
    private readonly CategoryTreeViewInfo _categoryInfo;
    public string CategoryName => _categoryInfo.Name;

    public TreeViewModelCategory(CategoryTreeViewInfo categoryInfo, IDesktopFileIconPathRefiner iconPathRefiner, ILogger logger, IRootRequirer rootRequirer, ILocalizationProvider localizationProvider) : base(logger, rootRequirer, localizationProvider)
    {
        _categoryInfo = categoryInfo;
        var children = categoryInfo.Headers.Select(header => new TreeViewModelDesktopFile(header.Path,
            header.IconPath!,
            header.Name!, 
            iconPathRefiner,
            logger, 
            rootRequirer,
            localizationProvider){Parent = this});
        Children.AddRange(children);
        
    }

    public override void SendSelectionChangeMessage()
    {
        this.Parent!.SendSelectionChangeMessage();
        var message = new UpdateSelectedCategoryMessage()
        {
            CategoryName = CategoryName,
        };
        MessageBus.Current.SendMessage(message);
    }
}