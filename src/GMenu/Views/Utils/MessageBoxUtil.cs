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
}