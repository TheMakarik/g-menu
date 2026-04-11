namespace GMenu.Modules.LinuxSystem.interfaces;

public interface IRootRequirer
{
    public Task RequireRootAsync(CancellationTokenSource cancellationTokenSource);
}