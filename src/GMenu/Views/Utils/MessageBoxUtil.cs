namespace GMenu.Views.Utils;

public static class MessageBoxUtil
{
    public static async Task ShowCannotFindDirectoryBoxAsync(string path, ILocalizationProvider localizationProvider, Window owner)
    {
        var messageBox = new MessageBoxCustomParams
        {
            Icon = MsBox.Avalonia.Enums.Icon.Warning,
            ContentTitle = localizationProvider["Error"],
            ContentMessage = $"{localizationProvider["FileOrCatalogNotFound"]}: {path}",
            ShowInCenter = true,
            ButtonDefinitions =
            [new ButtonDefinition()
            {
                Name = localizationProvider["Ok"]
            }],
            CloseOnClickAway = true
        };

        await MessageBoxManager.GetMessageBoxCustom(messageBox).ShowWindowDialogAsync(owner);
    }

    public static void ShowCannotLoadThemeMessageLazy(Window? mainWindow)
    {
        if (mainWindow is null)
            return;

        mainWindow.Loaded += async (_, _) =>
        {

            var currentObservableConfiguration = App.Services.GetRequiredService<IConfigurationProvider>().CurrentObservable;
            
            if(!currentObservableConfiguration.ShowCannotLoadThemeFromDBusMessage)
                return;
            
            var viewModel = mainWindow.DataContext as ViewModelBase;
            if (viewModel is null)
                return;

            var messageBox = new MessageBoxCustomParams
            {
                Icon = Icon.Error,
                ContentTitle = viewModel.LocalizationProvider["Error"],
                ContentMessage = viewModel.LocalizationProvider["CannotLoadTheme"],
                ShowInCenter = true,
                ButtonDefinitions =
                [
                    new ButtonDefinition()
                    {
                        Name = viewModel.LocalizationProvider["Ok"]
                    },
                    new ButtonDefinition()
                    {
                        Name = viewModel.LocalizationProvider["DoNotShowAgain"]
                    }
                ]
            };

            var result =  await MessageBoxManager.GetMessageBoxCustom(messageBox).ShowWindowDialogAsync(mainWindow);

            if (result == viewModel.LocalizationProvider["DoNotShowAgain"])
               currentObservableConfiguration.ShowCannotLoadThemeFromDBusMessage = false;
        };
    }
}