using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public class TreeViewModelDesktopFile(string filePath, string iconPath, string name, IDesktopFileIconPathRefiner iconPathRefiner, ILogger logger, IRootRequirer rootRequirer) : TreeViewModelBase(logger, rootRequirer)
{
    public string? IconPath => iconPathRefiner.RefinePath(iconPath);
    public string Name => name;
}