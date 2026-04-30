using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GMenu.Views;

public partial class InfoWindow : ReactiveWindow<InfoWindowViewModel>
{
    public InfoWindow()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<InfoWindowViewModel>();
    }
}