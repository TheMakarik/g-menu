using System.Reactive.Disposables;
using System.Text;
using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    [Reactive] private string? _categoryName;
    [Reactive] private string? _directoryPath;
    [Reactive] private string? _desktopFileName;
    [Reactive] private int _desktopFilesCount;
    [Reactive] private string? _directoryLocalizationKey;

    private CompositeDisposable _disposables = new CompositeDisposable();

    public MainWindowViewModel(ILogger logger,
        ILocalizationProvider localizationProvider,
        IRootRequirer rootRequirer) : base(logger, rootRequirer, localizationProvider)
    {
        MessageBus.Current.Listen<SetDesktopFilesCountMessage>()
                 .Subscribe(message => DesktopFilesCount  = message.FilesCount );
     
             MessageBus.Current.Listen<UpdateSelectedDirectoryMessage>()
                 .Subscribe(message =>
                 {
                     DirectoryPath = message.Path;
                     DirectoryLocalizationKey = message.LocalizationKey;
                     CategoryName = null;
                     DesktopFileName = null;
                 })
                 .DisposeWith(_disposables);
     
             MessageBus.Current.Listen<UpdateSelectedCategoryMessage>()
                 .Subscribe(message =>
                 {
                     CategoryName = message.CategoryName;
                     DesktopFileName = null;
                 })
                 .DisposeWith(_disposables);

             
             MessageBus.Current.Listen<UpdateSelectedDesktopFileMessage>()
                 .Subscribe(message => { DesktopFileName = message.Name; })
                 .DisposeWith(_disposables);
             
         }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}