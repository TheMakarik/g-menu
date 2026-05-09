namespace GMenu.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    [Reactive] private int _desktopFilesCount;
    [Reactive] private string? _searchText;
    
    private TreeViewModelBase? _selectedItem;
    
    public Interaction<string, Unit> OpenLink { get; init; } = new();
    public Interaction<Unit, Unit> NothingToDelete { get; init; }  = new();
    public Interaction<Unit, bool> EnsureFileDeleteAction { get; init; }  = new();
    public Interaction<Unit, bool> EnsureCategoryDeleteAction { get; init; }  = new();
    public Interaction<string, Unit> ShowInTextEditor { get; init; }  = new();
    
    private readonly ILogger _logger;
    private readonly GMenuOptions _options;

    public MainWindowViewModel(ILogger logger,
        ILocalizationProvider localizationProvider,
        GMenuOptions options) : base(localizationProvider)
    {
        _logger = logger;
        _options = options;
        MessageBus.Current.Listen<SetDesktopFilesCountMessage>()
                 .Subscribe(message => DesktopFilesCount  = message.FilesCount );
        this
            .WhenPropertyChanged(e => e.SearchText)
            .Subscribe(onNext =>
                MessageBus.Current.SendMessage(new UpdateSearchTextMessage(){NewText = onNext.Value}));
        
        MessageBus.Current.Listen<SendSelectedItemMessage>()
            .Subscribe(message =>
            {
                _selectedItem = message.SelectedItem;
                logger.Information("Selected item: {CategoryCategoryName}", 
                    _selectedItem is TreeViewModelCategory category 
                        ? $"Category: {category.CategoryName}"
                        : _selectedItem is TreeViewModelDesktopFile file 
                                  ? $"File: {file.FilePath}"
                                  : "Unknown"
                    );
            });
        
        MessageBus.Current
            .Listen<RemoveTreeViewItem>()
            .Subscribe(async message => await ExecuteRemovingAsync(message.ItemToRemove));
        
        
        MessageBus.Current
            .Listen<RaiseShowInTextEditorInteraction>()
            .Subscribe(async message =>
            {
                await ShowInTextEditor.Handle(message.Path);
            });
        
    }

    [ReactiveCommand]
    private void OpenAboutDesktopFilesLink()
    {
        OpenLink.Handle(_options.Core.AboutDesktopFiles);
    }

    [ReactiveCommand]
    private async Task RemoveSelectedAsync()
    {
        if (_selectedItem is null)
        {
            _logger.Debug("Nothing to delete");
            await NothingToDelete.Handle(Unit.Default);
        }
        await ExecuteRemovingAsync(_selectedItem);
        
    }

    [ReactiveCommand]
    private void SendGoUpInDesktopFilesViewMessage()
    {
        MessageBus.Current.SendMessage(new GoUpInDesktopFilesViewMessage());
    }
    
    [ReactiveCommand]
    private void SendGoDownInDesktopFilesViewMessage()
    {
        MessageBus.Current.SendMessage(new GoDownInDesktopFilesViewMessage());   
    }

    private async Task ExecuteRemovingAsync(TreeViewModelBase? selectedItem)
    {
        var result = await (_selectedItem is TreeViewModelDesktopFile
            ? EnsureFileDeleteAction.Handle(Unit.Default) 
            : EnsureCategoryDeleteAction.Handle(Unit.Default));

    }
}