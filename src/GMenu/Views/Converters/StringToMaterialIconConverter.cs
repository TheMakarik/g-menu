namespace GMenu.Views.Converters;

public sealed class StringToMaterialIconConverter : MarkupExtension, IValueConverter
{
    [DynamicDependency( DynamicallyAccessedMemberTypes.All, typeof(MaterialIconKind))]
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string kindString)
            return null; 
        
        return Enum.TryParse<MaterialIconKind>(kindString, true, out var kindEnum)
            ? new MaterialIcon(){Kind = kindEnum} 
            : null;
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