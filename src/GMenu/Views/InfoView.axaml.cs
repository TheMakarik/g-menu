using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GMenu.Views;

public partial class InfoView : ReactiveWindow<InfoViewModel>
{
    public InfoView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<InfoViewModel>();
    }
}