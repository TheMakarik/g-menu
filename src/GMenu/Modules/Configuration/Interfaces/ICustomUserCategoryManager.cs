namespace GMenu.Modules.Configuration.Interfaces;

public interface ICustomUserCategoryManager
{
    public void Add(CustomUserCategory customUserCategory);
    public void Remove(CustomUserCategory customUserCategory);
    public IReadOnlyCollection<CustomUserCategory>? GetAll();
}