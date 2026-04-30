using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;

namespace GMenu.Views;

public sealed partial class SelectFilesWindow : ReactiveWindow<SelectFilesWindowViewModel>
{
    public string[] Paths { get; }

    public SelectFilesWindow(string[] paths)
    {
        Paths = paths;
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<SelectFilesWindowViewModel>();
        ViewModel!.OpenDirectory.RegisterHandler(async context =>
        {
            var topLevel = GetTopLevel(this);
            Debug.Assert(topLevel is not null);
            await topLevel.Launcher.LaunchDirectoryInfoAsync(new DirectoryInfo(context.Input));
            context.SetOutput(Unit.Default);
        });
    }
    
}