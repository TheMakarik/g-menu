using GMenu.ViewModels.Messages;
using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public sealed partial class TreeViewDirectoriesViewModel(
    IGrouping<string, DesktopFileHeader> headers,
    DirectoryTreeViewInfo directoryInfo,
    ILogger logger,
    IRootRequirer rootRequirer) : ViewModelBase(logger, rootRequirer)
{

    public string Path => directoryInfo.Path;
    [Reactive] private ObservableCollection<DesktopFileHeader> _children = [];
    [Reactive] private bool _isOpen;

    [ReactiveCommand]
    private void RegisterNewDirectory()
    {
        MessageBus.Current.SendMessage(new RegisterNewDirectoryMessage());
        logger.Debug("Registered new directory from another directory using directory menu");
    }

    [ReactiveCommand]
    private void RemoveSelf()
    {
        MessageBus.Current.SendMessage(new RemoveTreeViewElementMessage<TreeViewDirectoriesViewModel>(this));
    }
}