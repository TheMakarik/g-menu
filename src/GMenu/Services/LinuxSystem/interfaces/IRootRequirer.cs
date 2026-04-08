using System.Threading;
using System.Threading.Tasks;

namespace GMenu.Services.LinuxSystem.interfaces;

public interface IRootRequirer
{
    public Task RequireRootAsync(CancellationTokenSource cancellationTokenSource);
}