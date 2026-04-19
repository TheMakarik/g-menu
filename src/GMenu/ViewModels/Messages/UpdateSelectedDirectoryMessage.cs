namespace GMenu.ViewModels.Messages;

public class UpdateSelectedDirectoryMessage
{
    public required string Path { get; init; }
    public required string? LocalizationKey { get; init; }
}