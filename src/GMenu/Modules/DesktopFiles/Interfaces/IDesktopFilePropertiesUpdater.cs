namespace GMenu.Modules.DesktopFiles.Interfaces;

public interface IDesktopFilePropertiesUpdater
{
    public void RequireEntryUpdate(string key, bool value);
    public void RequireEntryUpdate(string key, string value);
    public bool HasUnexecutedUpdates();
    public void ClearProperties();
    public Task ExecuteUpdate(string path);
}