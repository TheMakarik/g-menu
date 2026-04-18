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
            }
        };

        this.Content = image;
    }
}