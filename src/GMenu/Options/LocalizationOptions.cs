namespace GMenu.Options;

public sealed class LocalizationOptions
{
    public required CultureInfo DefaultCulture { get; set; }
    public required List<CultureInfo> SupportedCultures { get; set; }
    public required string LocalizationPath { get; set; }
    public required string NotFoundValue { get; set; }
}