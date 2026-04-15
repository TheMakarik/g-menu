namespace GMenu.Modules.Configuration.Interfaces;

public interface IConfigurationProvider
{
    public ObservableConfiguration CurrentObservable { get; }
    public Task EnsureExistsAsync();
}