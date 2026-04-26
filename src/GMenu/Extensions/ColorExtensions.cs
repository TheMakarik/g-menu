using Material.Colors;

namespace GMenu.Extensions;

public static class ColorExtensions
{
    extension(GMenu.Modules.ColorUtils.Enums.Color color)
    {
        public PrimaryColor? GetMaterialPrimaryColor() =>
            color switch
            {
                GMenu.Modules.ColorUtils.Enums.Color.Red => PrimaryColor.Red,
                GMenu.Modules.ColorUtils.Enums.Color.Pink => PrimaryColor.Pink,
                GMenu.Modules.ColorUtils.Enums.Color.Purple => PrimaryColor.Purple,
                GMenu.Modules.ColorUtils.Enums.Color.DeepPurple => PrimaryColor.DeepPurple,
                GMenu.Modules.ColorUtils.Enums.Color.Indigo => PrimaryColor.Indigo,
                GMenu.Modules.ColorUtils.Enums.Color.Blue => PrimaryColor.Blue,
                GMenu.Modules.ColorUtils.Enums.Color.LightBlue => PrimaryColor.LightBlue,
                GMenu.Modules.ColorUtils.Enums.Color.Cyan => PrimaryColor.Cyan,
                GMenu.Modules.ColorUtils.Enums.Color.Teal => PrimaryColor.Teal,
                GMenu.Modules.ColorUtils.Enums.Color.Green => PrimaryColor.Green,
                GMenu.Modules.ColorUtils.Enums.Color.LightGreen => PrimaryColor.LightGreen,
                GMenu.Modules.ColorUtils.Enums.Color.Lime => PrimaryColor.Lime,
                GMenu.Modules.ColorUtils.Enums.Color.Yellow => PrimaryColor.Yellow,
                GMenu.Modules.ColorUtils.Enums.Color.Amber => PrimaryColor.Amber,
                GMenu.Modules.ColorUtils.Enums.Color.Orange => PrimaryColor.Orange,
                GMenu.Modules.ColorUtils.Enums.Color.DeepOrange => PrimaryColor.DeepOrange,
                GMenu.Modules.ColorUtils.Enums.Color.Brown => PrimaryColor.Brown,
                GMenu.Modules.ColorUtils.Enums.Color.Grey => PrimaryColor.Grey,
                GMenu.Modules.ColorUtils.Enums.Color.BlueGrey => PrimaryColor.BlueGrey,
                _ => null
            };

        public SecondaryColor? GetMaterialSecondaryColor() =>
            color switch
            {
                GMenu.Modules.ColorUtils.Enums.Color.Red => SecondaryColor.Red,
                GMenu.Modules.ColorUtils.Enums.Color.Pink => SecondaryColor.Pink,
                GMenu.Modules.ColorUtils.Enums.Color.Purple => SecondaryColor.Purple,
                GMenu.Modules.ColorUtils.Enums.Color.DeepPurple => SecondaryColor.DeepPurple,
                GMenu.Modules.ColorUtils.Enums.Color.Indigo => SecondaryColor.Indigo,
                GMenu.Modules.ColorUtils.Enums.Color.Blue => SecondaryColor.Blue,
                GMenu.Modules.ColorUtils.Enums.Color.LightBlue => SecondaryColor.LightBlue,
                GMenu.Modules.ColorUtils.Enums.Color.Cyan => SecondaryColor.Cyan,
                GMenu.Modules.ColorUtils.Enums.Color.Teal => SecondaryColor.Teal,
                GMenu.Modules.ColorUtils.Enums.Color.Green => SecondaryColor.Green,
                GMenu.Modules.ColorUtils.Enums.Color.LightGreen => SecondaryColor.LightGreen,
                GMenu.Modules.ColorUtils.Enums.Color.Lime => SecondaryColor.Lime,
                GMenu.Modules.ColorUtils.Enums.Color.Yellow => SecondaryColor.Yellow,
                GMenu.Modules.ColorUtils.Enums.Color.Amber => SecondaryColor.Amber,
                GMenu.Modules.ColorUtils.Enums.Color.Orange => SecondaryColor.Orange,
                GMenu.Modules.ColorUtils.Enums.Color.DeepOrange => SecondaryColor.DeepOrange,
                _ => null
            };
    }
}