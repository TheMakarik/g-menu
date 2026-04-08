using System.Threading.Tasks;
using GMenu.Models.Common;
using GMenu.Models.GNOME;

namespace GMenu.Services.LinuxSystem.interfaces;

public interface IGNOMEThemeLoader
{
    public string GetThemeHex();
}