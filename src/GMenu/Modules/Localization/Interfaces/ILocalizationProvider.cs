using System.Globalization;
using GMenu.Modules.Localization.Model;

namespace GMenu.Modules.Localization.Interfaces;

public interface ILocalizationProvider
{
    public void SetLocalization(CultureInfo cultureInfo);
    public string this[LocalizationKey key] { get; }
}