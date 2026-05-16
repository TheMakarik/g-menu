namespace GMenu.Modules.DesktopFiles;

public sealed class DesktopFilePropertiesUpdater(ILogger logger) : IDesktopFilePropertiesUpdater
{
    private ConcurrentDictionary<string, string> _propertiesToUpdate = new();
    
    public void RequireEntryUpdate(string key, bool value)
    {
        _propertiesToUpdate.AddOrUpdate(key, value.ToString().ToLower(), (_, _) => value.ToString().ToLower());
        logger.Debug("Added {key}:{value} to update waiting", key, value);
    }

    public void RequireEntryUpdate(string key, string value)
    {
        _propertiesToUpdate.AddOrUpdate(key, value, (_, _) => value.ToString().ToLower());
        
#if DEBUG
        logger.Debug("Added {key}:{value} to update waiting", key, value);
#endif
    }

    public bool HasUnexecutedUpdates()
    {
        return _propertiesToUpdate.Any();
    }

    public void ClearProperties()
    {
        var count = _propertiesToUpdate.Count;
        _propertiesToUpdate.Clear();
        logger.Debug("{count} Properties deleted", count);
    }


    public Task ExecuteUpdate(string path)
    {
        throw new NotImplementedException();
    }
}