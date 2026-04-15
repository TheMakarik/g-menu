using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public sealed partial class MainWindowViewModel(
    ILogger logger,
    IDesktopFileHeaderReader reader,
    IRootRequirer rootRequirer) : ViewModelBase(logger, rootRequirer)
{
    
}