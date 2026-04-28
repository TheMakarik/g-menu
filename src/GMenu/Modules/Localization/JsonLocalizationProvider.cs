namespace GMenu.Modules.Localization;

public sealed class JsonLocalizationProvider(ILogger logger, GMenuOptions options) : ILocalizationProvider
{
    private FrozenDictionary<string, string>? _values = null;
    
    public void SetLocalization(CultureInfo cultureInfo)
    {
        if (_values is not null)
        {
            logger.Information("Setting localization to {culture} requires to restart", cultureInfo);
            return;
        }

         using var stream = File.Open(Path.ChangeExtension(Path.Combine(options.Localization.LocalizationPath, cultureInfo.ToString()), ".json"), FileMode.Open);
#pragma warning disable IL2026
#pragma warning disable IL3050
        _values = (JsonSerializer
            .Deserialize<Dictionary<string, string>>(stream, DictionarySerializerContext.Default.Options))
#pragma warning restore IL2026
#pragma warning restore IL3050
            .ToFrozenDictionary();

        logger.Information("Localization loaded as {culture}", cultureInfo);
    }

    public string this[string key] => _values!.TryGetValue(key, out var result) ? result : options.Localization.NotFoundValue;
}