namespace GMenu.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    [Reactive] private int _desktopFilesCount;
    [Reactive] private string? _searchText;
    
    public Interaction<string, Unit> OpenLink = new();
    private readonly GMenuOptions _options;

    public MainWindowViewModel(ILogger logger,
        ILocalizationProvider localizationProvider,
        GMenuOptions options) : base(localizationProvider)
    {
        _options = options;
        MessageBus.Current.Listen<SetDesktopFilesCountMessage>()
                 .Subscribe(message => DesktopFilesCount  = message.FilesCount );
        this
            .WhenPropertyChanged(e => e.SearchText)
            .Subscribe(onNext =>
                MessageBus.Current.SendMessage(new UpdateSearchTextMessage(){NewText = onNext.Value}));

    }

    [ReactiveCommand]
    private void OpenAboutDesktopFilesLink()
    {
        OpenLink.Handle(_options.Core.AboutDesktopFiles);
    }
    
}