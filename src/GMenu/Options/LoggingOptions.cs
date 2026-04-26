namespace GMenu.Options;

public sealed class LoggingOptions
{
    public required string OutputTemplate { get; set; }
    public required string LogsDirectory { get; set; }
    public required string LogFileNamePrefix { get; set; }
    public required ConsoleTheme SerilogConsoleTheme { get; set; }
    public RollingInterval RollingInterval { get; set; }
}