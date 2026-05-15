namespace GMenu.Modules.DesktopFiles.Interfaces;

public interface IDesktopFileRemover
{
    public ValueTask RemoveAsync(string filePath);
}