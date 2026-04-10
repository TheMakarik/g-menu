using System.Threading.Tasks;
using GMenu.Modules.Configuration.Model;

namespace GMenu.Modules.Configuration.Interfaces;

public interface IConfiguration
{
    public ObservableConfiguration GetObservable();
    public Task EnsureExistsAsync();
}