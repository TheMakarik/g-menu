namespace GMenu.ViewModels.Messages;

public sealed class SetDesktopFileToEditMessage
{
    public required string? Path { get; init; }
}