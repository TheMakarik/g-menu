using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public class TreeViewModelCategory : TreeViewModelBase
{
    private readonly CategoryTreeViewInfo _categoryInfo;

    public TreeViewModelCategory(CategoryTreeViewInfo categoryInfo, IDesktopFileIconPathRefiner iconPathRefiner, ILogger logger, IRootRequirer rootRequirer) : base(logger, rootRequirer)
    {
        _categoryInfo = categoryInfo;
        var children = categoryInfo.Headers.Select(header => new TreeViewModelDesktopFile(header.Path, header.IconPath!, header.Name!, iconPathRefiner, logger, rootRequirer));
        Children.AddRange(children);
    }

    public string CategoryName => _categoryInfo.Name;
}