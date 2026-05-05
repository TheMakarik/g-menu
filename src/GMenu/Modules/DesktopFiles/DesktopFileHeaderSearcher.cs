namespace GMenu.Modules.DesktopFiles;

public sealed class DesktopFileHeaderSearcher : IDesktopFilesHeaderSearcher
{

    public bool IsSearchValue(string namePattern, DesktopFileHeader header)
    {
       var result  = header.Name.Contains(namePattern.Trim(), StringComparison.OrdinalIgnoreCase);
       if(header.Name.ToUpper() == "TINT")
           Console.WriteLine($"{namePattern}");
       return result;
    }
}