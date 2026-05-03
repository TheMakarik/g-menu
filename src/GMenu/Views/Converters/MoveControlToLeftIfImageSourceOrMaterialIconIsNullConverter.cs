using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace GMenu.Views.Converters;

public class MoveControlToLeftIfImageSourceOrMaterialIconIsNullConverter : MarkupExtension, IValueConverter
{
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not Thickness margin)
            return new Thickness(0);

        if (value is IImage or MaterialIcon)
            return margin;

        var iconSize = (double)(Application.Current!.Resources["IconInTreeViewSize"] ?? throw new InvalidOperationException($"IconInTreeViewSize is not set"));
        return new Thickness(margin.Left - iconSize, margin.Top, margin.Right, margin.Bottom);
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