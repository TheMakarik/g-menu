
namespace GMenu.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    
    public MainWindow()
    {
        InitializeComponent();
    }

    public async void OpenInfo(object sender, RoutedEventArgs args)
    {
       await new InfoView().ShowDialog(this);
    }
    
}