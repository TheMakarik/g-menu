namespace GMenu.Modules.Localization.Interfaces;

public interface ILocalizationProvider
{
    public Task SetLocalizationAsync(CultureInfo cultureInfo);
    public string this[string key] { get; }
}