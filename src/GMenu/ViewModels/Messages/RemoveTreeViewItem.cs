namespace GMenu.ViewModels.Messages;

public sealed class RemoveTreeViewItem
{
    public required TreeViewModelBase? ItemToRemove { get; set; }
}