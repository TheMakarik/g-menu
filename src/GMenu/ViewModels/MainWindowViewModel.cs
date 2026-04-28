namespace GMenu.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    [Reactive] private int _desktopFilesCount;
    
    private CompositeDisposable _disposables = new CompositeDisposable();

    public Interaction<string, Unit> OpenLink = new();
    private readonly GMenuOptions _options;

    public MainWindowViewModel(ILogger logger,
        ILocalizationProvider localizationProvider,
        GMenuOptions options,
        IRootRequirer rootRequirer) : base(logger, rootRequirer, localizationProvider)
    {
        _options = options;
        MessageBus.Current.Listen<SetDesktopFilesCountMessage>()
                 .Subscribe(message => DesktopFilesCount  = message.FilesCount );
     
             
         }

    [ReactiveCommand]
    private void OpenAboutDesktopFilesLink()
    {
        OpenLink.Handle(_options.Core.AboutDesktopFiles);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}