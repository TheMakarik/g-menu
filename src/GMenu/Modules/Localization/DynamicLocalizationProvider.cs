using System;
using System.Globalization;
using System.Threading;
using GMenu.Modules.Localization.Interfaces;
using GMenu.Modules.Localization.Model;

namespace GMenu.Modules.Localization;

public class DynamicLocalizationProvider(
    Action<CultureInfo, CultureInfo> updateUIOnCultureChanging,
    Func<LocalizationKey, string> getStringDynamic
    ) : ILocalizationProvider
{
    private readonly Lock _lock = new Lock();
    private CultureInfo _currentCulture = CultureInfo.CurrentUICulture;
    
    public void SetLocalization(CultureInfo cultureInfo)
    {
        using var scope = _lock.EnterScope();
        var oldCultureName = _currentCulture.ToString(); //To avoid reference at _currentCulture that will be updated 
        _currentCulture = cultureInfo;
        updateUIOnCultureChanging(_currentCulture, new CultureInfo(oldCultureName));
    }

    public string this[LocalizationKey key] => getStringDynamic(key);
}