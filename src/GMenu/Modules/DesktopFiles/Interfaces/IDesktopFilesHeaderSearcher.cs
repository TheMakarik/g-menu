namespace GMenu.Modules.DesktopFiles.Interfaces;

public interface IDesktopFilesHeaderSearcher
{
    public IEnumerable<DesktopFileHeader> Search(string namePattern, IEnumerable<DesktopFileHeader> headers);
}