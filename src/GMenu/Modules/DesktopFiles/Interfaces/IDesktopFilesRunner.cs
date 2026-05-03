namespace GMenu.Modules.DesktopFiles.Interfaces;

public interface IDesktopFilesRunner
{
    public ValueTask RunDesktopFileFromHeaderAsync(DesktopFileHeader header, bool requireSudo, CancellationTokenSource source);
}