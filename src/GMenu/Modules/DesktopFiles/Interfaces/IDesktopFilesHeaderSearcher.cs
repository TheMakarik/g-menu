namespace GMenu.Modules.DesktopFiles.Interfaces;

public interface IDesktopFilesHeaderSearcher
{
    public bool IsSearchValue(string namePattern, DesktopFileHeader header);
}