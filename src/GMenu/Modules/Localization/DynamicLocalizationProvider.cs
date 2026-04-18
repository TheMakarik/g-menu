using ILogger = Serilog.ILogger;

namespace GMenu.Modules.Localization;

public class DynamicLocalizationProvider(
    Action<CultureInfo> updateUIOnCultureChanging,
    Func<string, CultureInfo, string> getStringDynamic,
    ILogger logger
    ) : ILocalizationProvider
{
    private readonly Lock _lock = new Lock();
    private CultureInfo _currentCulture = CultureInfo.CurrentUICulture;
    
    public void SetLocalization(CultureInfo cultureInfo)
    {
        using var scope = _lock.EnterScope();
        logger.Information("Setting localization to {culture}", cultureInfo);
        _currentCulture = cultureInfo;
        updateUIOnCultureChanging(cultureInfo);
    }

    public string this[string key]
    {
        get
        {
            var result = getStringDynamic(key, _currentCulture);
            return result;
        }
    }
}