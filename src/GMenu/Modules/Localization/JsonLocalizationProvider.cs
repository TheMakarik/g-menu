namespace GMenu.Modules.Localization;

public class JsonLocalizationProvider(ILogger logger, GMenuOptions options) : ILocalizationProvider
{
    private FrozenDictionary<string, string>? _values = null;
    
    public async Task SetLocalizationAsync(CultureInfo cultureInfo)
    {
        if (_values is not null)
        {
            logger.Information("Setting localization to {culture} requires to restart", cultureInfo);
            return;
        }

        await using var stream = File.Open(options.Localization.LocalizationPath, FileMode.Open);
#pragma warning disable IL2026
#pragma warning disable IL3050
        _values = (await JsonSerializer
            .DeserializeAsync<Dictionary<string, string>>(stream, DictionarySerializerContext.Default.Options))
#pragma warning restore IL2026
#pragma warning restore IL3050
            .ToFrozenDictionary();

        logger.Information("Localization loaded as {culture}", cultureInfo);
    }

    public string this[string key] => _values![key];
}