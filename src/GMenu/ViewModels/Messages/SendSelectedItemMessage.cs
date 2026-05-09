namespace GMenu.ViewModels.Messages;

public sealed class SendSelectedItemMessage
{
    public required TreeViewModelBase SelectedItem { get; set; }
}