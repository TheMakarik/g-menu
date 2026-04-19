using System.Text;

namespace GMenu.Views.Converters;

public class AddStringSeparatorConverter : MarkupExtension, IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 4)
            return null;
        
        var pathLocalizationKey = values[1] as string;
        var category = values[2] as string;
        var fileName = values[3] as string;

        if (values[0] is not string path)
            return null;
        
        var directory = (string) new TryLocalizeDirectoryConverter().Convert([pathLocalizationKey, path], targetType, parameter, culture)!;

        var stringBuilder = new StringBuilder();
        var isFirst = true;
        foreach (var directoryPath in directory!.Split(Path.DirectorySeparatorChar))
        {
            if(!isFirst)
                stringBuilder.Append(StaticConfiguration.PathInFooterSeparator);
            
            stringBuilder.Append(directoryPath);

            if (isFirst)
                isFirst = false;
        }

        if(category is null)
            return stringBuilder.ToString();
        
        var localizedCategory = new TryLocalizeValueConverter().Convert(category, targetType, parameter, culture)!;
        stringBuilder.Append(StaticConfiguration.PathInFooterSeparator);
        stringBuilder.Append(localizedCategory);

        if (fileName is null)
            return stringBuilder.ToString();
        
        stringBuilder.Append(StaticConfiguration.PathInFooterSeparator);
        stringBuilder.Append(fileName);
        
        return stringBuilder.ToString();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}