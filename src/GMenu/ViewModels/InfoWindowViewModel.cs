namespace GMenu.ViewModels;

public class InfoWindowViewModel(GMenuOptions options, ILogger logger, IRootRequirer requirerRoot, ILocalizationProvider localizationProvider) : ViewModelBase(logger, requirerRoot, localizationProvider)
{
    public string Version => options.Core.Version.ToString();
    public string Github => options.Core.Github;
}