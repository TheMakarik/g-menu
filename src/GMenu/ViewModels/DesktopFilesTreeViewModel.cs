
namespace GMenu.ViewModels;

public sealed partial class DesktopFilesTreeViewModel(
    IDesktopFileHeaderReader reader,
    ILogger logger,
    IDesktopFilesHeaderSearcher searcher,
    IDesktopFileIconPathRefiner iconPathRefiner,
    IDesktopFilesRunner desktopFilesRunner,
    ICustomUserCategoryManager customUserCategoryManager,
    ILocalizationProvider localizationProvider)
    : ViewModelBase(localizationProvider)
{
    [Reactive] private ObservableCollection<TreeViewModelBase> _children = [];
    [Reactive] private string? _searchText;
    [Reactive] private ObservableCollection<TreeViewModelDesktopFile> _searchResults = [];
    
    private CancellationTokenSource _searchCancellationTokenSource = new CancellationTokenSource();

    public Interaction<Unit, Unit> GoDownInTheDesktopFilesView { get; } = new();
    public Interaction<Unit, Unit> GoUpInTheDesktopFilesView { get; } = new();
    
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
            .GroupBy(static header =>
                string.IsNullOrEmpty(header.Category) ? StaticConfiguration.Uncategorized : header.Category)
            .Select(group => new TreeViewModelCategory(
                new CategoryTreeViewInfo
                {
                    Name = group.Key!,
                    Icon = group.Key!,
                    Headers = group
                },
                desktopFilesRunner,
                iconPathRefiner,
                logger,
                LocalizationProvider)
            {
                Parent = null
            }).Union((customUserCategoryManager.GetAll() ?? []).Select(category => new TreeViewModelCategory(
                new CategoryTreeViewInfo()
                {
                    Name = category.LocalizedName,
                    Icon = category.IconKind ?? category.Name,
                    Headers = null
                },
                desktopFilesRunner,
                iconPathRefiner,
                logger,
                LocalizationProvider)))
            .OrderBy(category => category.CategoryName);
        
        _children.Clear();
        _children.AddRange(groupedByCategory);
        
        logger.Information("Created {count} categories", _children.Count);

        MessageBus.Current
            .Listen<UpdateSearchTextMessage>()
            .Subscribe(message =>
        {
            SearchText = message.NewText;
        });
        
        MessageBus.Current
            .Listen<GoDownInDesktopFilesViewMessage>()
            .Subscribe(async _ => await GoDownInTheDesktopFilesView.Handle(Unit.Default));
        
        MessageBus.Current
            .Listen<GoUpInDesktopFilesViewMessage>()
            .Subscribe(async _ => await GoUpInTheDesktopFilesView.Handle(Unit.Default));

        MessageBus.Current
            .Listen<GetCategoriesListMessage>()
            .Subscribe(message => message.SetCategoriesListAction(this.Children
                .Cast<TreeViewModelCategory>()
                .Select(category => category.CategoryName)
                .ToList()));

        MessageBus.Current
            .Listen<AddCategoryMessage>()
            .Subscribe(message =>
            {
                _children.Add(new TreeViewModelCategory(
                    message.Category,
                    desktopFilesRunner,
                    iconPathRefiner,
                    logger,
                    LocalizationProvider));

                _children = new ObservableCollection<TreeViewModelBase>(
                    _children
                         .Cast<TreeViewModelCategory>()
                         .OrderBy(category => category.CategoryName));
            });

        MessageBus.Current.Listen<RemoveCategoryMessage>()
            .Subscribe(message =>
            {

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
        logger.Debug("Searching with pattern {p} and found {c} files categories", SearchText, counter);
#endif
    }
}