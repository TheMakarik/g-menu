namespace GMenu.Modules.DesktopFiles;

public  sealed class DesktopFileReader : IDesktopFileReader
{
    public IEnumerable<string> ReadEntry(string path)
    {
        return File.ReadLines(path)
            .SkipWhile(static line => line != DesktopFileKeys.DesktopEntryHeader)
            .Where(static line => !string.IsNullOrEmpty(line))
            .Skip(1) // skip entry header
            .TakeWhile(static line => line[0] is not '[');
    }

    public IAsyncEnumerable<string> ReadEntryAsync(string path)
    {
        return File.ReadLinesAsync(path)
            .SkipWhile(static line => line != DesktopFileKeys.DesktopEntryHeader)
            .Where(static line => !string.IsNullOrEmpty(line))
            .Skip(1) // skip entry header
            .TakeWhile(static line => line[0] is not '[');
    }
}