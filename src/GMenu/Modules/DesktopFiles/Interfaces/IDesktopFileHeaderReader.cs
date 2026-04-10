using System.Collections.Generic;
using System.Threading.Tasks;
using GMenu.Models.DesktopFiles;

namespace GMenu.Modules.DesktopFiles.Interfaces;

public interface IDesktopFileHeaderReader
{
    public Task<IReadOnlyCollection<DesktopFileHeader>> GetAllHeaders();
}