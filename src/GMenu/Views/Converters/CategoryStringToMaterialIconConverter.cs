namespace GMenu.Views.Converters;


public sealed class CategoryStringToMaterialIconConverter : MarkupExtension, IValueConverter
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicFields, typeof(MaterialIconKind))]
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is not string category)
            return null;

        return App.Services.GetRequiredService<GMenuOptions>().CategoriesIcons.TryGetValue(category, out var iconKind)
            ? new MaterialIcon(){Kind = Enum.Parse<MaterialIconKind>(iconKind)}
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