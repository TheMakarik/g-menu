using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    
    [Reactive] private int _desktopFilesCount;
    
    public MainWindowViewModel(ILogger logger,
        ILocalizationProvider localizationProvider,
        IRootRequirer rootRequirer) : base(logger, rootRequirer, localizationProvider)
    {
        MessageBus.Current.Listen<SetDesktopFilesCountMessage>()
            .Subscribe(message => DesktopFilesCount  = message.FilesCount );
        
    }
}