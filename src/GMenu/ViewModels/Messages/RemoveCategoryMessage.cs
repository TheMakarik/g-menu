namespace GMenu.ViewModels.Messages;

public sealed class RemoveCategoryMessage
{
    public required TreeViewModelCategory Category { get; init; }
}