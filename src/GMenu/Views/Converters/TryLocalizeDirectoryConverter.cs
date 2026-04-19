namespace GMenu.Views.Converters;

public class TryLocalizeDirectoryConverter : MarkupExtension, IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 2)
            return null;
        
        var key = values[0] as string;
        var path = values[1] as string;
        
        if (string.IsNullOrEmpty(path))
            return null;
        
        if (!string.IsNullOrEmpty(key))
        {
            var localizationProvider = App.Services.GetRequiredService<ILocalizationProvider>();
            var localizedValue = localizationProvider[key];

            if (localizedValue != StaticConfiguration.CannotFoundKeyInLocalizationValue)
                return localizedValue;
        }
        
        var configurationProvider = App.Services.GetRequiredService<IConfigurationProvider>();
        
        var allParents = configurationProvider
            .CurrentObservable
            .SearchDesktopFilesDirectories
            .Where(parent => !string.IsNullOrEmpty(parent.LocalizationKey) && 
                             !string.IsNullOrEmpty(parent.Path) &&
                             path.StartsWith(parent.Path))
            .ToList();
        
        var bestParent = allParents
            .OrderByDescending(parent => parent.Path.Length)
            .FirstOrDefault();

        if (bestParent.LocalizationKey is null)
            return path;

        var localization = App.Services.GetRequiredService<ILocalizationProvider>();
        var parentLocalized = localization[bestParent.LocalizationKey];

        if (parentLocalized == StaticConfiguration.CannotFoundKeyInLocalizationValue)
            return path;
        
        var subPath = path[bestParent.Path.Length..].TrimStart('/', '\\');
        
        return string.IsNullOrEmpty(subPath) 
            ? parentLocalized 
            : $"{parentLocalized}/{subPath}";
    }
    

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}