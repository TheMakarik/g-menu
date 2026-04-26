namespace GMenu.Modules.DesktopFiles.Interfaces;

public interface IDesktopFileIconPathRefiner
{
    public string? RefinePath(string? path, IReadOnlyCollection<string> pathsToRefineIcon);
    public void StartBackgroundIconsLoading(IEnumerable<string> iconInDesktopFiles, IReadOnlyCollection<string> pathsToRefineIcon);
}