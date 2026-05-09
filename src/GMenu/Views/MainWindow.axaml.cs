using GMenu.Views.Utils;

namespace GMenu.Views;


public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = App.Services.GetRequiredService<MainWindowViewModel>();
        ViewModel!.NothingToDelete.RegisterHandler(async context =>
        {
            var messageBox = new MessageBoxStandardParams
            {
                ButtonDefinitions = ButtonEnum.Ok,
                ContentTitle = ViewModel!.LocalizationProvider["NothingToDeleteTitle"],
                ContentMessage = ViewModel!.LocalizationProvider["NothingToDelete"],
                Icon = MsBox.Avalonia.Enums.Icon.Info
            };
            await MessageBoxManager.GetMessageBoxStandard(messageBox).ShowWindowDialogAsync(this);
            context.SetOutput(Unit.Default);
        });

        this.ViewModel!.ShowInTextEditor.RegisterHandler(async context =>
        {
            var topLevel = GetTopLevel(this);
            Debug.Assert(topLevel is not null);

            if (!File.Exists(context.Input))
                await MessageBoxUtil.ShowCannotFindDirectoryBoxAsync(context.Input, ViewModel!.LocalizationProvider, this);
            
            var success = await topLevel.Launcher.LaunchFileInfoAsync(new FileInfo(context.Input));
            
            if(!success)
                await MessageBoxUtil.ShowCannotFindDirectoryBoxAsync(context.Input, ViewModel!.LocalizationProvider, this);
            
            context.SetOutput(Unit.Default);
        });

        ViewModel!.EnsureCategoryDeleteAction.RegisterHandler(async context =>
        {
            var messageBox = new MessageBoxCustomParams()
            {
                ButtonDefinitions = [
                    new ButtonDefinition()
                    {
                        IsDefault = true, 
                        Name = ViewModel!.LocalizationProvider["No"]
                    },
                    new ButtonDefinition()
                    {
                        Name = ViewModel!.LocalizationProvider["Yes"]
                    },
                ],
                ContentTitle = ViewModel!.LocalizationProvider["EnsureAction"],
                ContentMessage = ViewModel!.LocalizationProvider["WantToDeleteCategory"],
                Icon = MsBox.Avalonia.Enums.Icon.Question
            };
            var result = await MessageBoxManager.GetMessageBoxCustom(messageBox).ShowWindowDialogAsync(this);
            context.SetOutput(result == ViewModel!.LocalizationProvider["Yes"]);
        });
        
        ViewModel!.EnsureFileDeleteAction.RegisterHandler(async context =>
        {
            var messageBox = new MessageBoxCustomParams()
            {
                ButtonDefinitions = [
                    new ButtonDefinition()
                    {
                        IsDefault = true, 
                        Name = ViewModel!.LocalizationProvider["No"]
                    },
                    new ButtonDefinition()
                    {
                        Name = ViewModel!.LocalizationProvider["Yes"]
                    },
                ],
                ContentTitle = ViewModel!.LocalizationProvider["EnsureAction"],
                ContentMessage = ViewModel!.LocalizationProvider["WantToDeleteFile"],
                Icon = MsBox.Avalonia.Enums.Icon.Question
            };
            var result = await MessageBoxManager.GetMessageBoxCustom(messageBox).ShowWindowDialogAsync(this);
            context.SetOutput(result == ViewModel!.LocalizationProvider["Yes"]);
        });
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

    private void LoadInnerLoftContentForegroundUpdating(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var materialUnderLine = (MaterialUnderline)textBox.GetTemplateDescendants().First(visual => visual is MaterialUnderline);
        materialUnderLine.GetPropertyChangedObservable(MaterialUnderline.IsActiveProperty)
            .Subscribe(args =>
            {
                var innerLeftContent = textBox.InnerLeftContent as TemplatedControl;
                innerLeftContent?.Foreground = args.NewValue is true 
                    ? materialUnderLine.ActiveBrush
                    : materialUnderLine.IdleBrush;
            });
    }
}