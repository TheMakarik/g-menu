using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public partial class TreeViewModelBase(ILogger logger, IRootRequirer rootRequirer, ILocalizationProvider localizationProvider) : ViewModelBase(logger, rootRequirer, localizationProvider)
{
    [Reactive] private ObservableCollection<TreeViewModelBase> _children = [];
}