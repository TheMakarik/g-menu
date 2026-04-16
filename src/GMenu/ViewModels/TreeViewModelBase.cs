using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public partial class TreeViewModelBase(ILogger logger, IRootRequirer rootRequirer) : ViewModelBase(logger, rootRequirer)
{
    [Reactive] private ObservableCollection<TreeViewModelBase> _children = [];
}