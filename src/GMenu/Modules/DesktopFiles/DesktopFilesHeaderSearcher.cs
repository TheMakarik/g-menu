namespace GMenu.Modules.DesktopFiles;

public class DesktopFilesHeaderSearcher(ILogger logger) : IDesktopFilesHeaderSearcher
{
    public IEnumerable<DesktopFileHeader> Search(string namePattern, IEnumerable<DesktopFileHeader> headers)
    {
        return headers
            .AsParallel()
            .Where(header => !header.IsDummy)
            .Where(header => header.Name is not null)
            .Where(header => header.Name!.ContainsRange(namePattern.AsSpan()));
        
    }
}