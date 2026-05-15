namespace GMenu.Modules.DesktopFiles.Interfaces;

public interface IDesktopFilesExecFormatter
{
    public string PrepareCommand(ReadOnlySpan<char> headerExec);
    public IReadOnlyList<string> ParseCommandLine(string commandLine);
    public string EscapeForShSingleQuotes(string argument);
}