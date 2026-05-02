namespace GMenu.Views.Controls;

public class DesktopFileImage : ContentControl
{
    public static readonly DirectProperty<DesktopFileImage, string> SourceStringProperty = AvaloniaProperty.RegisterDirect<DesktopFileImage, string>(
        nameof(SourceString), o => o.SourceString, (o, v) => o.SourceString = v);

    public string SourceString
    {
        get;
        set
        {
            if (SetAndRaise(SourceStringProperty, ref field, value))
                UpdateContentControl();
        }
    } = string.Empty;

    private void UpdateContentControl()
    {
        var extension = Path.GetExtension(this.SourceString);
        var image = new Image
        {
            Width = this.Width,
            Height = this.Height,
            Stretch = Stretch.Fill,
            Source = extension switch
            {
                ".png" or ".jpg" or ".jpeg" => File.Exists(this.SourceString) 
                    ? new Bitmap(this.SourceString) 
                    : null,
                ".svg" => File.Exists(this.SourceString)
                    ? new SvgImage() { Source = SvgSource.Load(this.SourceString)  }
                    : null,
                _ => null
            }
        };

        this.Content = image;
    }
}