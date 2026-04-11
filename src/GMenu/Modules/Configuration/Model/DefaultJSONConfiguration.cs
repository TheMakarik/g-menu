namespace GMenu.Modules.Configuration.Model;

public sealed class DefaultJSONConfiguration(
    User user,
    DesktopFileDirectory[] defaultDesktopFileDirectories,
    List<UnexistingCategory> unexistingCategory)
{
    public User User { get; } = user;
    public DesktopFileDirectory[] DefaultDesktopFileDirectories { get; } = defaultDesktopFileDirectories;
    public List<UnexistingCategory> UnexistingCategory { get; } = unexistingCategory;
}