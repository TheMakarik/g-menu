using Avalonia.Data.Converters;
using Material.Icons.Avalonia;

namespace GMenu.Views.Converters;

public sealed class CategoryStringToMaterialIconConverter : MarkupExtension, IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is not string category)
            return null;

        return StaticConfiguration.MaterialIconsForCategory.TryGetValue(category, out Material.Icons.MaterialIconKind iconKind)
            ? new MaterialIcon(){Kind = iconKind}
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