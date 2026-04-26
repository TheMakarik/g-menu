namespace GMenu.Modules.DesktopFiles.Interfaces;

public interface IDesktopFileHeaderReader
{
    public IReadOnlyCollection<DesktopFileHeader> GetAllHeaders(string[] paths);
}