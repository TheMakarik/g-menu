namespace GMenu.Modules.DesktopFiles;

public sealed class DesktopFileHeaderSearcher : IDesktopFilesHeaderSearcher
{

    public bool IsSearchValue(string namePattern, DesktopFileHeader header)
    {
        var pattern = namePattern.Trim();
        var result = header.Name.Contains(pattern, StringComparison.InvariantCultureIgnoreCase)
                     || (header.UnlocalizedName?.Contains(pattern, StringComparison.InvariantCultureIgnoreCase) ?? false)
                     || (header.Exec?.Contains(pattern, StringComparison.InvariantCultureIgnoreCase) ?? false)
                     || (header.Directory.Contains(pattern, StringComparison.InvariantCultureIgnoreCase));
        return result;
    }
}