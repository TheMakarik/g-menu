namespace GMenu.Modules.DesktopFiles;

public sealed class DesktopFileHeaderSearcher : IDesktopFilesHeaderSearcher
{

    public bool IsSearchValue(string namePattern, DesktopFileHeader header)
    {
        return header.Name.Contains(namePattern, StringComparison.InvariantCultureIgnoreCase)
               || (header.UnlocalizedName?.Contains(namePattern, StringComparison.InvariantCultureIgnoreCase) ?? false)
               || (header.Category?.Contains(namePattern, StringComparison.InvariantCultureIgnoreCase) ?? false)
               || (header.Exec?.Contains(namePattern, StringComparison.InvariantCultureIgnoreCase) ?? false);
    }
}