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
    [Reactive] private ObservableCollection<TreeViewModelBase> _searchResults = [];
    
    
    private SemaphoreSlim _searchSemaphore = new(1, 1);
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
                logger,
                localizationProvider)
            {
                Parent = null
            });
        
        _children.Clear();
        _children.AddRange(groupedByCategory);
        
        _logger.Information("Created {count} categories", _children.Count);

        this.WhenPropertyChanged(property => property.SearchText)
            .Subscribe(onNext =>
            {
                _searchCancellationTokenSource.Cancel();
                if (onNext.Value is null)
                    return;
                
                _searchSemaphore.Wait();
                try
                {
                    _searchResults.Clear();
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error while searching");
                }
                finally
                {
                    _searchSemaphore.Release();
                }
                BeginSearch();
            });
    }

  
    private void BeginSearch()
    {
        var searchChildren = Children
            .SelectMany(children => children.Children)
            .Cast<TreeViewModelDesktopFile>();
        Debug.Assert(SearchText is not null);
        Parallel.ForEach(searchChildren, (item, state) =>
        {
            if (_searchCancellationTokenSource.Token.CanBeCanceled)
               state.Break();

            if (!searcher.IsSearchValue(SearchText, item.Header))
                return;

            _searchSemaphore.Wait();
            try
            {
                _searchResults.Add(item);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while searching");
            }
            finally
            {
                _searchSemaphore.Release();
            }
        });
    }

}