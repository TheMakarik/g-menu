namespace GMenu.ViewModels;

public class InfoWindowViewModel(GMenuOptions options, ILocalizationProvider localizationProvider) : ViewModelBase(localizationProvider)
{
    public string Version => options.Core.Version.ToString();
    public string Github => options.Core.Github;
}