using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public sealed partial class DesktopFilesTreeViewModel(
    IDesktopFileHeaderReader reader, ILogger logger, 
    IRootRequirer rootRequirer) : ViewModelBase(logger, rootRequirer)
{
    [Reactive] private ObservableCollection<TreeViewDirectoriesViewModel> _children = [];

    [ReactiveCommand]
    private async Task LoadDesktopFilesAsync()
    {
        var result = await WithRootRequire(Observable.Start(reader.GetAllHeaders), nameof(reader.GetAllHeaders));
        _children
            .AddRange(result
                .GroupBy(header => header.Directory)
                .Select(header => new TreeViewDirectoriesViewModel(header, new DirectoryTreeViewInfo()
                {
                    Path = header.First().Directory
                }, logger, rootRequirer)));
    }
}