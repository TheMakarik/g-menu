namespace GMenu.Modules.DesktopFiles;

public class DesktopFilesHeaderSearcher(ILogger logger) : IDesktopFilesHeaderSearcher
{
    public IEnumerable<DesktopFileHeader> Search(Span<char> namePattern, IEnumerable<DesktopFileHeader> headers)
    {
        headers
            .Where(header => !header.IsDummy)
            .Where(header => header.Name is not null)
            .Where(header => header.Name.ContainsRange(namePattern))
    }
}