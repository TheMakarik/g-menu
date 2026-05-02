namespace GMenu.Modules.DesktopFiles.Interfaces;

public interface IDesktopFilesRunner
{
    public Task RunDesktopFileAsync(string path, bool requireSudo);
}