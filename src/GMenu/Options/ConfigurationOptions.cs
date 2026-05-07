namespace GMenu.Options;

public sealed class ConfigurationOptions
{
    public required string Directory { get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), field); set; }
    public required string ConfigFileName { get; set; }
    public required List<string> ValidIconExtensions { get; set; }
}