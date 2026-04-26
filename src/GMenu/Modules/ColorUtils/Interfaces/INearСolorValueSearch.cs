namespace GMenu.Modules.ColorUtils.Interfaces;

using Color = GMenu.Modules.ColorUtils.Enums.Color;


public interface INearСolorValueSearch
{
    public Color GetNearColor(Rgb rgb);
}