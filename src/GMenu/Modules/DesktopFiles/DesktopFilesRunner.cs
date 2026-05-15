
namespace GMenu.Modules.DesktopFiles;

public sealed partial class DesktopFilesRunner(
    GMenuOptions options,
    ILogger logger,
    ILinuxTerminalLauncher terminalLauncher,
    IDesktopFileReader reader,
    IDesktopFilesExecFormatter formatter) : IDesktopFilesRunner    
{
    public async ValueTask RunDesktopFileFromHeaderAsync(DesktopFileHeader header, bool requireSudo, CancellationTokenSource source)
    {
        try
        {
            if (header.Exec is null)
            {
                logger.Error("Trying to run broken desktop file without exec: {path}", header.Path);
                return;
            }

            var terminal = await ParseTerminalKeyAsync(header.Path);
            var command = formatter.PrepareCommand(header.Exec.AsSpan());

            if (string.IsNullOrWhiteSpace(command))
            {
                logger.Error("Prepared command is empty for: {path}", header.Path);
                return;
            }

            if (requireSudo)
                command = $"{Path.Join(Environment.CurrentDirectory, options.Linux.ShellScripts.ExecuteWithPolicyKit)} {formatter.EscapeForShSingleQuotes(command)}";

            if (terminal)
            {
                await terminalLauncher.LaunchTerminalAsync(command);
                logger.Debug("Run in terminal with command: {command}", command);
                return;
            }
            
            var args = formatter.ParseCommandLine(command);
            if (args.Count == 0)
            {
                logger.Error("Failed to parse command line: {command}", command);
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = args[0],
                UseShellExecute = false,
                CreateNoWindow = false,
            };
            foreach (var arg in args.Skip(1))
                    startInfo.ArgumentList.Add(arg);

            Process.Start(startInfo);
            logger.Debug("Run process with command: {command}", command);
        }
        catch (OperationCanceledException)
        {
            logger.Warning("Run app from desktop file {path} with sudo mode was cancel", header.Path);
        }
    }

    private void RedirectOutput(Process? process)
    {
        if (process is null)
            return;

        Task.Factory.StartNew(() =>
        {
            try
            {
                while (!process.StandardOutput.EndOfStream)
                {
                    var line = process.StandardOutput.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    logger.Debug("App ({ProcessProcessName}) output: {Line}", process.ProcessName, line);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Error occurred");
                throw;
            }
           
        }, TaskCreationOptions.LongRunning);
    }

    private async Task<bool> ParseTerminalKeyAsync(string path)
    {
        await foreach (var line in reader.ReadEntryAsync(path))
        {
            var equalsIndex = line.AsSpan().IndexOf('=');
            if (equalsIndex == -1) continue;
            var key = line.AsSpan(0, equalsIndex);
            if (!key.SequenceEqual(DesktopFileKeys.TerminalKey.AsSpan())) continue;
            var value = line.AsSpan(equalsIndex + 1);
            if (bool.TryParse(value, out var terminal))
                return terminal;
            logger.Error("Can't parse Terminal value {value}", value.ToString());
            return false;
        }
        return false;
    }
}