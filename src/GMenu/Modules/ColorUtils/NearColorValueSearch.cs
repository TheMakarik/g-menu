using GMenu.Modules.ColorUtils.Interfaces;
using Color = GMenu.Modules.ColorUtils.Enums.Color;

namespace GMenu.Modules.ColorUtils;

public class NearColorValueSearch : INearСolorValueSearch
{
    public Color GetNearColor(Rgb rgb)
    {
        switch (rgb.Red)
        {
            case 0 when rgb is { Green: 0, Blue: 0 }:
                return Color.Black;
            case 255 when rgb is { Green: 255, Blue: 255 }:
                return Color.White;
        }

        if (rgb.Red > rgb.Green && rgb.Red > rgb.Blue)
        {
            return rgb.Green switch
            {
                < 100 when rgb.Blue < 100 => Color.Red,
                > 150 when rgb.Blue > 150 => Color.Pink,
                _ => rgb.Green > rgb.Blue ? Color.Orange : Color.Red
            };
        }
        
        if (rgb.Green > rgb.Red && rgb.Green > rgb.Blue)
        {
            return rgb.Red switch
            {
                < 100 when rgb.Blue < 100 => Color.Green,
                > 200 when rgb.Blue > 200 => Color.Lime,
                _ => rgb.Red > rgb.Blue ? Color.LightGreen : Color.Green
            };
        }
        
        if (rgb.Blue > rgb.Red && rgb.Blue > rgb.Green)
        {
            switch (rgb.Red)
            {
                case < 100 when rgb.Green < 100:
                    return Color.Blue;
                case > 150 when rgb.Green > 150:
                    return Color.Cyan;
            }

            return rgb.Red > rgb.Green ? Color.Purple : Color.Blue;
        }

        if (Math.Abs(rgb.Red - rgb.Green) >= 30 || Math.Abs(rgb.Green - rgb.Blue) >= 30)
            return rgb.Red switch
            {
                > 200 when rgb.Green > 200 && rgb.Blue < 100 => Color.Yellow,
                > 200 when rgb.Green > 150 && rgb.Blue < 50 => Color.Amber,
                _ => Color.Grey
            };
        if (rgb.Red < 50) return Color.Black;
        return rgb.Red < 150 ? Color.Grey : Color.White;

    }
}