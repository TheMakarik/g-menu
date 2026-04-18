namespace GMenu.Views.Converters;

public class TryLocalizeValueConverter : MarkupExtension, IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string valueToLocalize)
            return value;
        
        var result =  App.Services.GetRequiredService<ILocalizationProvider>()[valueToLocalize];

        return result == StaticConfiguration.CannotFoundKeyInLocalizationValue
            ? valueToLocalize
            : result;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}