using System.Threading;
using System.Threading.Tasks;

namespace GMenu.Modules.LinuxSystem.interfaces;

public interface IRootRequirer
{
    public Task RequireRootAsync(CancellationTokenSource cancellationTokenSource);
}