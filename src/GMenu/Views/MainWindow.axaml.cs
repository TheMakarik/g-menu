
namespace GMenu.Views;


public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public async void OpenInfo(object sender, RoutedEventArgs args)
    {
       await new InfoWindow().ShowDialog(this);
    }

    private async void OpenDesktopFilesDirectories(object? sender, RoutedEventArgs e)
    {
        await (new SelectFilesWindow(StaticConfiguration.PathToDesktopFiles)).ShowDialog(this);
    }
    
    private async void OpenDesktopFileIconsDirectories(object? sender, RoutedEventArgs e)
    {
        await (new SelectFilesWindow(StaticConfiguration.PathsToRefineIcon)).ShowDialog(this);
    }
    
    private async void OpenFreedesktopDirectories(object? sender, RoutedEventArgs e)
    {
        await (new SelectFilesWindow(StaticConfiguration.FreedesktopDataDirectories)).ShowDialog(this);
    }
}