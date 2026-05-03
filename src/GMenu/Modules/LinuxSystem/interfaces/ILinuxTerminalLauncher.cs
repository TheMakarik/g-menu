namespace GMenu.Modules.LinuxSystem.interfaces;

public interface ILinuxTerminalLauncher
{
    public Task LaunchTerminalAsync(string command);
}