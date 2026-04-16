using GMenu.ViewModels.Messages;
using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public sealed partial class TreeViewModelDirectories : TreeViewModelBase
{
    private readonly DirectoryTreeViewInfo _directoryInfo;
    private readonly ILogger _logger;
    public string Path => _directoryInfo.Path;

    public TreeViewModelDirectories(
        DirectoryTreeViewInfo directoryInfo,
        IDesktopFileIconPathRefiner iconPathRefiner,
        ILogger logger,
        IRootRequirer rootRequirer) : base(logger, rootRequirer)
    {
        _directoryInfo = directoryInfo;
        _logger = logger;

        var groupedByCategoryHeaders = directoryInfo.Headers
            .Where(header => header.Category is not null)
            .Where(header => !header.IsBroken)
            .GroupBy(header => header.Category)
            .Select(group => new TreeViewModelCategory(
                new CategoryTreeViewInfo
                {
                    Path = Path,
                    Name = group.Key!,
                    Headers = group!
                },
                iconPathRefiner,
                logger,
                rootRequirer));
        
        Children.AddRange(groupedByCategoryHeaders);
    }

  
 
    [ReactiveCommand]
    private void RegisterNewDirectory()
    {
        MessageBus.Current.SendMessage(new RegisterNewDirectoryMessage());
        _logger.Debug("Registered new directory from another directory using directory menu");
    }

    [ReactiveCommand]
    private void RemoveSelf()
    {
        MessageBus.Current.SendMessage(new RemoveTreeViewElementMessage<TreeViewModelDirectories>(this));
    }
}