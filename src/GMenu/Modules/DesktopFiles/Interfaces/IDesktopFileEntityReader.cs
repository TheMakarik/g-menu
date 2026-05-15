namespace GMenu.Modules.DesktopFiles.Interfaces;

public interface IDesktopFileEntityReader
{
    Task<DesktopFile?> ReadDesktopFileAsync(string path, CancellationToken cancellationToken = default);
}
