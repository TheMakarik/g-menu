using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GMenu.Models.DesktopFiles;
using GMenu.Modules.DesktopFiles.Interfaces;
using Serilog;

namespace GMenu.Modules.DesktopFiles;

public sealed class DesktopFileHeaderReader(ILogger logger) : IDesktopFileHeaderReader
{
    const string CategoriesPrefix = "Catergories";
    
    public Task<IReadOnlyCollection<DesktopFileHeader>> GetAllHeaders()
    {
        var count = 0;
        throw new Exception();
    }
}