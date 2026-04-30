namespace GMenu.Views;

public partial class InfoWindow : ReactiveWindow<InfoWindowViewModel>
{
    public InfoWindow()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<InfoWindowViewModel>();
    }
}