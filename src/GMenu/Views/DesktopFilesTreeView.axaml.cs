using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GMenu.Views;

public partial class DesktopFilesTreeView : ReactiveUserControl<DesktopFilesTreeViewModel>
{
    public DesktopFilesTreeView()
    {
        InitializeComponent();
        
        DataContext = App.Services.GetRequiredService<DesktopFilesTreeViewModel>();
        
    }
}