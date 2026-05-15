namespace GMenu.ViewModels.Messages;

public sealed class GetCategoriesListMessage
{
    public required Action<ICollection<string>> SetCategoriesListAction { get; set; }
}