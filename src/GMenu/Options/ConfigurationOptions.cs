namespace GMenu.Options;

public sealed class ConfigurationOptions
{
    public required string Directory { get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), field); set; }
    public required string ConfigFileName { get => Path.Combine(Directory, field); set; }
    public required List<string> ValidIconExtensions { get; set; }
}