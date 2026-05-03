namespace GMenu.Modules.DesktopFiles.Interfaces;

public interface IDesktopFileReader
{
    public IEnumerable<string> ReadEntry(string path);
    public IAsyncEnumerable<string> ReadEntryAsync(string path);
}