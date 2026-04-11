namespace GMenu.Modules.DesktopFiles.Interfaces;

public interface IDesktopFileHeaderReader
{
    public Task<IReadOnlyCollection<DesktopFileHeader>> GetAllHeadersAsync();
}