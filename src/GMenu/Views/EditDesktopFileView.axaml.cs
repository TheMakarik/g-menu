using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GMenu.Views;

public partial class EditDesktopFileView : ReactiveUserControl<EditDesktopFilesViewModel>
{
    public EditDesktopFileView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<EditDesktopFilesViewModel>();
    }
}