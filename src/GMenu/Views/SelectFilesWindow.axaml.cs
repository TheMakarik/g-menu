using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Models;

namespace GMenu.Views;


public sealed partial class SelectFilesWindow : ReactiveWindow<SelectFilesWindowViewModel>
{
    public IReadOnlyCollection<string> Paths { get; }

    [DynamicDependency(DynamicallyAccessedMemberTypes.AllEvents, typeof(SelectFilesWindow))]
    public SelectFilesWindow(IReadOnlyCollection<string> paths)
    {
        Paths = paths;
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<SelectFilesWindowViewModel>();
        ViewModel!.OpenDirectory.RegisterHandler(async context =>
        {
            context.SetOutput(Unit.Default);
            var topLevel = GetTopLevel(this);
            Debug.Assert(topLevel is not null);

            if (!Directory.Exists(context.Input))
            {
                ShowCannotFindDirectoryBox(context.Input);
                return;
            }
            
            var success = await topLevel.Launcher.LaunchDirectoryInfoAsync(new DirectoryInfo(context.Input));
            if (!success)
            {
                
            }
        });
    }

    private void ShowCannotFindDirectoryBox(string path)
    {
        var messageBox = new MessageBoxCustomParams();
        messageBox.Icon = MsBox.Avalonia.Enums.Icon.Warning;
        messageBox.ContentTitle = ViewModel!.LocalizationProvider["Error"];
        messageBox.ContentMessage = $"{ViewModel.LocalizationProvider["FileOrCatalogNotFound"]}: {path}";
        messageBox.ShowInCenter = true;
        messageBox.ButtonDefinitions = [new ButtonDefinition()
        {
            Name = ViewModel!.LocalizationProvider["Ok"]
        }];
        messageBox.CloseOnClickAway = true;
        
    }
}