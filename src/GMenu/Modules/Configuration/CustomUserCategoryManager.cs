namespace GMenu.Modules.Configuration;

public sealed class CustomUserCategoryManager(IConfigurationProvider configurationProvider) : ICustomUserCategoryManager
{
    public void Add(CustomUserCategory customUserCategory)
    {
        configurationProvider
            .CurrentObservable
            .CustomCategories?.Add(customUserCategory);
    }

    public void Remove(CustomUserCategory customUserCategory)
    {
        configurationProvider
            .CurrentObservable
            .CustomCategories?.Remove(customUserCategory);
    }

    public IReadOnlyCollection<CustomUserCategory>? GetAll()
    {
        return configurationProvider
            .CurrentObservable
            .CustomCategories;
    }
}