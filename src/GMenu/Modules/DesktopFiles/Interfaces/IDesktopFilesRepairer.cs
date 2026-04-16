namespace GMenu.Modules.DesktopFiles.Interfaces;

public interface IDesktopFilesRepairer
{
    public void IsBroken(DesktopFileHeader header);
    public bool TryRepairAsync(string path);
}