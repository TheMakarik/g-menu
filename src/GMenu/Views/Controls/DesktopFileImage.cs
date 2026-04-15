using Avalonia.Media.Imaging;
using Avalonia.Svg.Skia;
using GMenu.Views.SpecificModel;

namespace GMenu.Views.Controls;

public class DesktopFileImage : ContentControl
{
    
    private string _sourceString = string.Empty;

    public static readonly DirectProperty<DesktopFileImage, string> SourceStringProperty = AvaloniaProperty.RegisterDirect<DesktopFileImage, string>(
        nameof(SourceString), o => o.SourceString, (o, v) => o.SourceString = v);

    public string SourceString
    {
        get => _sourceString;
        set
        {
            if(SetAndRaise(SourceStringProperty, ref _sourceString, value))
                 UpdateContentControl();
            
        }
    }

    private void UpdateContentControl()
    {
        var extension = Path.GetExtension(this.SourceString);
        this.Content = extension switch
        {
            ".png" or ".jpg" or ".jpeg" => new Image() { Source = new Bitmap(_sourceString), Stretch = Stretch.Fill },
            ".svg" => new SvgImage() { Source = new SvgSource(new Uri(_sourceString, UriKind.Absolute)) },
            ".xpm" => new Image(),
            _ => this.Content
        };
    }
}