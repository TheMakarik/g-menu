namespace GMenu.Modules.DesktopFiles.Interfaces;

public interface IDesktopFileIconPathRefiner
{
    public string? RefinePath(string? path);
    public void StartBackgroundIconsLoading(IEnumerable<string> iconInDesktopFiles);
}