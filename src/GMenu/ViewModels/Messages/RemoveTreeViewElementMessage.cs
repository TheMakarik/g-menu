namespace GMenu.ViewModels.Messages;

public sealed class RemoveTreeViewElementMessage<T>(T elementToDelete)
{
    public T ElementToDelete { get; } = elementToDelete;
}