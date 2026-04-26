namespace GMenu.Options;

public sealed class GMenuOptions
{
    public required ConfigurationOptions Configuration { get; set; }
    public required LoggingOptions Logging { get; set; }
    public required LocalizationOptions Localization { get; set; }
    public required UiOptions Ui { get; set; }
    public required CoreOptions Core { get; set; }
    public required Dictionary<string, string> CategoriesIcons { get; set; }
}