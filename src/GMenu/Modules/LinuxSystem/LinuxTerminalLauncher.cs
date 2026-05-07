namespace GMenu.Modules.LinuxSystem;

public sealed class LinuxTerminalLauncher(ILogger logger, GMenuOptions options) : ILinuxTerminalLauncher
{
    public async Task LaunchTerminalAsync(string command)
    {
        var terminals = options.Linux.SupportedTerminals;

        if (terminals.Count == 0)
        {
            logger.Error("SupportedTerminals list is empty or null");
            return;
        }
        
        logger.Information("Terminals available in config: {Count}", terminals.Count);
        
        var orderedTerminals = terminals.OrderBy(t => t.Name == "xdg-terminal-exec" ? 0 : 1);
        
        foreach (var terminal in orderedTerminals)
        {
            logger.Debug("Checking terminal: {Name}", terminal.Name);

            if (!await IsExecutableAvailableAsync(terminal.Name))
            {
                logger.Debug("Terminal {Name} not found in system, skipping", terminal.Name);
                continue;
            }

            logger.Information("Terminal {Name} found, attempting to launch", terminal.Name);

            try
            {
                var shellCommand = BuildTerminalLaunchCommand(command, terminal);
                
                logger.Debug("Launching terminal: {Terminal} with command: {Command}", terminal.Name, shellCommand);

                // Разбираем командную строку терминала на аргументы
                var args = ParseCommandLine(shellCommand);
                if (args.Count == 0)
                {
                    logger.Error("Failed to parse terminal command: {Command}", shellCommand);
                    continue;
                }

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = args[0],
                    UseShellExecute = false,
                    RedirectStandardError = false,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false,
                    CreateNoWindow = false,
                    Environment =
                    {
                        ["DISPLAY"] = Environment.GetEnvironmentVariable("DISPLAY") ?? string.Empty,
                        ["XAUTHORITY"] = Environment.GetEnvironmentVariable("XAUTHORITY") ??  string.Empty,
                        ["DBUS_SESSION_BUS_ADDRESS"] = Environment.GetEnvironmentVariable("DBUS_SESSION_BUS_ADDRESS") ??  string.Empty,
                        ["XDG_RUNTIME_DIR"] = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR") ??  string.Empty,
                        ["WAYLAND_DISPLAY"] = Environment.GetEnvironmentVariable("WAYLAND_DISPLAY") ??  string.Empty
                    }
                };

                foreach (var arg in args.Skip(1))
                    processStartInfo.ArgumentList.Add(arg);

                using var process = Process.Start(processStartInfo);
                
                if (process is null)
                {
                    logger.Error("Failed to start process for {Name}", terminal.Name);
                    continue;
                }

                process.EnableRaisingEvents = true;
                
                logger.Information("Terminal {Name} launched successfully (PID: {Pid})", terminal.Name, process.Id);
                return;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Unexpected error launching {Name}: {Message}", terminal.Name, ex.Message);
            }
        }

        logger.Error("No terminal could launch the command: {Command}", command);
    }
    
    private string BuildTerminalLaunchCommand(string userCommand, SupportedTerminal terminal)
    {
        var innerCommand = BuildInnerShellCommand(userCommand);
        var terminalCommand = terminal.Command.Replace("{0}", innerCommand);
        return $"{terminal.Name} {terminalCommand}";
    }
    
    private string BuildInnerShellCommand(string userCommand)
    {
        var escaped = userCommand.Replace("'", "'\\''");
        return $"sh -c '{escaped}; exec sh'";
    }
    
    private static List<string> ParseCommandLine(string commandLine)
    {
        var args = new List<string>();
        var current = new StringBuilder();
        var inQuote = false;
        var quoteChar = '\0';
        var i = 0;
        while (i < commandLine.Length)
        {
            var c = commandLine[i];
            switch (inQuote)
            {
                case false when (c == '"' || c == '\''):
                    inQuote = true;
                    quoteChar = c;
                    i++;
                    continue;
                case true when c == quoteChar:
                    inQuote = false;
                    quoteChar = '\0';
                    i++;
                    continue;
                case false when char.IsWhiteSpace(c):
                {
                    if (current.Length > 0)
                    {
                        args.Add(current.ToString());
                        current.Clear();
                    }
                    i++;
                    continue;
                }
                default:
                    current.Append(c);
                    i++;
                    break;
            }
        }
        if (current.Length > 0)
            args.Add(current.ToString());
        return args;
    }

    private async Task<bool> IsExecutableAvailableAsync(string name)
    {
        try
        {
            var whichPsi = new ProcessStartInfo
            {
                FileName = "which",
                Arguments = name,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var whichProcess = Process.Start(whichPsi);
            
            if (whichProcess is not null)
            {
                await whichProcess.WaitForExitAsync();
                if (whichProcess.ExitCode == 0)
                    return true;
            }
        }
        catch (Exception ex)
        {
            logger.Debug(ex, "Error checking availability of {Name} via which", name);
        }
        
        try
        {
            var commandPsi = new ProcessStartInfo
            {
                FileName = "sh",
                Arguments = $"-c \"command -v {name}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var commandProcess = Process.Start(commandPsi);
            
            if (commandProcess is not null)
            {
                await commandProcess.WaitForExitAsync();
                if (commandProcess.ExitCode == 0)
                    return true;
            }
        }
        catch (Exception ex)
        {
            logger.Debug(ex, "Error checking availability of {Name} via command -v", name);
        }

        try
        {
            var testProcessStartInfo = new ProcessStartInfo
            {
                FileName = name,
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var testProcess = Process.Start(testProcessStartInfo);

            if (testProcess is not null)
            {
                await testProcess.WaitForExitAsync();
                return true;
            }
        }
        catch (Exception e)
        {
            logger.Debug(e, "Error checking availability of {Name} via version test", name);
        }
        
        return false;
    }
}