namespace GMenu.Views.Converters;

public class ReverseBooleanConverter : MarkupExtension, IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool and false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool and false;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}