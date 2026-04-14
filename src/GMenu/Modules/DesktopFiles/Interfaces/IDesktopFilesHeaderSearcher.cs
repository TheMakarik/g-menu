namespace GMenu.Modules.DesktopFiles.Interfaces;

public interface IDesktopFilesHeaderSearcher
{
    public IEnumerable<DesktopFileHeader> Search(Span<char> namePattern, IEnumerable<DesktopFileHeader> headers);
}