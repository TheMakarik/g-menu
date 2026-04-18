using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public sealed partial class TreeViewModelDirectories : TreeViewModelBase
{
    private readonly DirectoryTreeViewInfo _directoryInfo;
    private readonly ILogger _logger;
    public string Path => _directoryInfo.Path;
    public string? LocalizationKey => _directoryInfo.LocalizationKey;

    public TreeViewModelDirectories(
        DirectoryTreeViewInfo directoryInfo,
        IDesktopFileIconPathRefiner iconPathRefiner,
        ILogger logger,
        ILocalizationProvider localizationProvider,
        IRootRequirer rootRequirer) : base(logger, rootRequirer, localizationProvider)
    {
        _directoryInfo = directoryInfo;
        _logger = logger;

        var groupedByCategoryHeaders = directoryInfo.Headers
            .Where(header => !header.IsBroken)
            .GroupBy(header => string.IsNullOrEmpty(header.Category) ? StaticConfiguration.UncategorizedCategory : header.Category)
            .OrderBy(group => group.Key)
            .Select(group => new TreeViewModelCategory(
                new CategoryTreeViewInfo
                {
                    Path = Path,
                    Name = group.Key!,
                    Headers = group!
                },
                iconPathRefiner,
                logger,
                rootRequirer, localizationProvider));
        
        Children.AddRange(groupedByCategoryHeaders);
    }

  
 
    [ReactiveCommand]
    private void RegisterNewDirectory()
    {
        MessageBus.Current.SendMessage(new RegisterNewDirectoryMessage());
    }

    [ReactiveCommand]
    private void RemoveSelf()
    {
        MessageBus.Current.SendMessage(new RemoveTreeViewElementMessage<TreeViewModelDirectories>(this));
    }
}