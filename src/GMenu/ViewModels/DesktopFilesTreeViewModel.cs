namespace GMenu.ViewModels;

public sealed partial class DesktopFilesTreeViewModel(
    IDesktopFileHeaderReader reader, 
    ILogger logger, 
    IConfigurationProvider configuration,
    IDesktopFileIconPathRefiner iconPathRefiner,
    ILocalizationProvider localizationProvider,
    IRootRequirer rootRequirer) : ViewModelBase(logger, rootRequirer, localizationProvider)
{
    [Reactive] private ObservableCollection<TreeViewModelBase> _children = [];
    
    public TreeViewModelBase? SelectedItem
    { 
        get => field;
        set
        {
            field?.IsSelected = false;
            value?.IsSelected = true;
            this.RaiseAndSetIfChanged(ref field, value);
        }
    }
    
    private readonly ILogger _logger = logger;
    private readonly IRootRequirer _rootRequirer = rootRequirer;

    [ReactiveCommand]
    private async Task LoadDesktopFilesAsync()
    {
        var result = await WithRootRequire(
            Observable.Start(() => reader.GetAllHeaders(StaticConfiguration.PathToDesktopFiles)), 
            nameof(reader.GetAllHeaders));
        
        iconPathRefiner.StartBackgroundIconsLoading(
            result.Where(header => header.IconPath is not null).Select(header => header.IconPath!), 
            StaticConfiguration.PathsToRefineIcon);
        
        MessageBus.Current.SendMessage(new SetDesktopFilesCountMessage(){FilesCount = result.Count});
        
        var groupedByCategory = result
            .Where(header => !header.IsBroken)
            .GroupBy(header => string.IsNullOrEmpty(header.Category) ? StaticConfiguration.Uncategorized : header.Category)
            .OrderBy(group => group.Key)
            .Select(group => new TreeViewModelCategory(
                new CategoryTreeViewInfo
                {
                    Path = group.First().Path,
                    Name = group.Key!,
                    Headers = group
                },
                iconPathRefiner,
                logger,
                rootRequirer, 
                localizationProvider)
            {
                Parent = null
            });
        
        _children.Clear();
        _children.AddRange(groupedByCategory);
        
        _logger.Information("Created {count} categories (no folders)", _children.Count);
    }
}