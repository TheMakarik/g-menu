
namespace GMenu.ViewModels;

public sealed partial class DesktopFilesTreeViewModel(
    IDesktopFileHeaderReader reader, 
    ILogger logger, 
    IDesktopFilesHeaderSearcher searcher,
    IDesktopFileIconPathRefiner iconPathRefiner,
    IDesktopFilesRunner desktopFilesRunner,
    ILocalizationProvider localizationProvider) : ViewModelBase(localizationProvider)
{
    [Reactive] private ObservableCollection<TreeViewModelBase> _children = [];
    [Reactive] private string? _searchText;
    [Reactive] private ObservableCollection<TreeViewModelDesktopFile> _searchResults = [];
    
    
    private CancellationTokenSource _searchCancellationTokenSource = new CancellationTokenSource();
    
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

    [ReactiveCommand]
    private async Task LoadDesktopFilesAsync()
    {
        var result = await Observable.Start(() => reader.GetAllHeaders(StaticConfiguration.PathToDesktopFiles));
        
        iconPathRefiner.StartBackgroundIconsLoading(
            result.Where(static header => header.IconPath is not null).Select(static header => header.IconPath!), 
            StaticConfiguration.PathsToRefineIcon);
        
        MessageBus.Current.SendMessage(new SetDesktopFilesCountMessage(){FilesCount = result.Count});
        
        var groupedByCategory = result
            .Where(static header => !header.IsBroken)
            .GroupBy(static header => string.IsNullOrEmpty(header.Category) ? StaticConfiguration.Uncategorized : header.Category)
            .OrderBy(static group => group.Key)
            .Select(group => new TreeViewModelCategory(
                new CategoryTreeViewInfo
                {
                    Path = group.First().Path,
                    Name = group.Key!,
                    Headers = group
                },
                desktopFilesRunner,
                iconPathRefiner,
                _logger,
                LocalizationProvider)
            {
                Parent = null
            });
        
        _children.Clear();
        _children.AddRange(groupedByCategory);
        
        _logger.Information("Created {count} categories", _children.Count);

        MessageBus.Current.Listen<UpdateSearchTextMessage>().Subscribe(message =>
        {
            SearchText = message.NewText;
        });

        this.WhenPropertyChanged(static property => property.SearchText)
            .Subscribe(onNext =>
            {
                RxSchedulers.MainThreadScheduler.Schedule(() => SearchResults.Clear());
                
                _searchCancellationTokenSource.Cancel();
                _searchCancellationTokenSource = new CancellationTokenSource();
                if (string.IsNullOrWhiteSpace(onNext.Value))
                    return;
                
                BeginSearch(_searchCancellationTokenSource.Token);
            });
    }


    private void BeginSearch(CancellationToken token = default)
    {
        var searchChildren = Children
            .SelectMany(static children => children.Children)
            .Cast<TreeViewModelDesktopFile>();
        Debug.Assert(SearchText is not null);
#if DEBUG
        var counter = 0;
#endif
       
        try
        {
            var searchValues = searchChildren.Where((child) =>
            {
                token.ThrowIfCancellationRequested();
                var isSearchValue = searcher.IsSearchValue(SearchText, child.Header);
#if DEBUG
                if(isSearchValue)
                  counter++;
#endif
                return isSearchValue;
            });
            
            RxSchedulers.MainThreadScheduler.Schedule(() =>
            {
                foreach (var value in searchValues)
                {
                    token.ThrowIfCancellationRequested();
                    SearchResults.Add(value);
                }
            });
        }
        catch (OperationCanceledException e)
        {
            RxSchedulers.MainThreadScheduler.Schedule(() => SearchResults.Clear());
        }
       
#if DEBUG
        _logger.Debug("Searching with pattern {p} and found {c} files categories", SearchText, counter);
#endif
    }
}