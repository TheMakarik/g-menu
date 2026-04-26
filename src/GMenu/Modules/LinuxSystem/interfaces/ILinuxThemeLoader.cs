namespace GMenu.Modules.LinuxSystem.interfaces;

public interface ILinuxThemeLoader
{
    public Task<Rgb?> GetThemeHexAsync();
}