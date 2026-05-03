namespace GMenu.Options;

public sealed class LinuxOptions
{
    public required ICollection<SupportedTerminal> SupportedTerminals { get; set; }
    public required ShellScripts ShellScripts { get; set; }
}