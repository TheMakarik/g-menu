using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public partial class TreeViewModelDesktopFile(string filePath, string iconPath, string name, 
    IDesktopFileIconPathRefiner iconPathRefiner, 
    ILogger logger,
    IRootRequirer rootRequirer,
    ILocalizationProvider localizationProvider) 
    : TreeViewModelBase(logger, rootRequirer, localizationProvider)
{
    public string? IconPath =>  iconPathRefiner.RefinePath(iconPath);
    public string Name => name;

}