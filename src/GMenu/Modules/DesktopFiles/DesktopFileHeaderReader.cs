namespace GMenu.Modules.DesktopFiles;

public sealed class DesktopFileHeaderReader(ILogger logger, IConfiguration configuration) : IDesktopFileHeaderReader
{
    const string CategoriesPrefix = "Catergories";
    
    public async Task<IReadOnlyCollection<DesktopFileHeader>> GetAllHeadersAsync(CancellationTokenSource cancellationTokenSource)
    {
        throw new NotImplementedException();
    }
}