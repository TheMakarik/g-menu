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

        logger.Information("Searching for a terminal to run command: {Command}", command);
        logger.Information("Terminals available in config: {Count}", terminals.Count);

        foreach (var terminal in terminals)
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
                var args = terminal.Command.Replace("{0}", command);
                
                logger.Debug("Launch arguments: {Args}", args);

                var proccessStartInfo = new ProcessStartInfo
                {
                    FileName = terminal.Name,
                    Arguments = args,
                    UseShellExecute = true,
                };

                using var process = Process.Start(proccessStartInfo);
                
                if (process is null)
                {
                    logger.Error("Failed to start process for {Name}", terminal.Name);
                    continue;
                }

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

    private async Task<bool> IsExecutableAvailableAsync(string name)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "which",
                Arguments = name,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var process = Process.Start(psi);
            
            if (process is null)
                return false;
            
            await process.WaitForExitAsync();
            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            logger.Debug(ex, "Error checking availability of {Name} via which", name);
            return false;
        }
    }
}