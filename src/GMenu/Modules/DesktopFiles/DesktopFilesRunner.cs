using System.Text.RegularExpressions;

namespace GMenu.Modules.DesktopFiles;

public sealed partial class DesktopFilesRunner(
    GMenuOptions options,
    ILogger logger,
    ILinuxTerminalLauncher terminalLauncher,
    IDesktopFileReader reader) : IDesktopFilesRunner    
{
    [GeneratedRegex(@"\s*%[fFuUiIcC]\s*")]
    private static partial Regex FieldCodesRegex();

    [GeneratedRegex(@"^(?:sh|bash)\s+-c\s+""(.+)""$")]
    private static partial Regex ShWrapperRegex();
    
    public async ValueTask RunDesktopFileFromHeaderAsync(DesktopFileHeader header, bool requireSudo, CancellationTokenSource source)
    {
        try
        {
            if (header.Exec is null)
            {
                logger.Error("Trying to run broken desktop file without exec: {path}", header.Path);
                return;
            }


            var terminal = false;
            await foreach(var line in reader.ReadEntryAsync(header.Path))
            {
                var equalsIndex = line.IndexOf('=');
                if (equalsIndex == -1)
                    continue;
                
                var key = line.AsSpan(0, equalsIndex);
                var value = line.AsSpan(equalsIndex + 1);

                if (key != DesktopFile.TerminalKey)
                    continue;

                if (!bool.TryParse(value, out terminal))
                {
                    logger.Error("Can't parse Terminal value {value}", value.ToString());
                    return;
                }
               
                break;
            }

            var command = PrepareExecToShellRun(header.Exec);

            var argumentSudoCommand = $"{options.Linux.ShellScripts.ExecuteWithPolicyKit} \"{command}\"";
            if(terminal)
                if(requireSudo)
                  await terminalLauncher.LaunchTerminalAsync(argumentSudoCommand);
                else 
                    await terminalLauncher.LaunchTerminalAsync(command);
            else
            {
                var startInfo = new ProcessStartInfo()
                {
                    FileName = "/bin/sh",
                    UseShellExecute = true,
                    Arguments = requireSudo ? argumentSudoCommand : $"-c {command}",
                };
                
                Process.Start(startInfo);
            }
            
            logger.Debug("Run process with start info command: {command}", command);
        }
        catch (OperationCanceledException)
        {
            logger.Warning("Run app from desktop file {path} with sudo mode was cancel", header.Path);
        }
       
    }

    private string PrepareExecToShellRun(string headerExec)
    {
        logger.Debug("Preparing Exec for shell run: {Exec}", headerExec);
        
        var command = FieldCodesRegex().Replace(headerExec, " ");
        
        var shMatch = ShWrapperRegex().Match(command);
        if (shMatch.Success)
        {
            command = shMatch.Groups[1].Value;
            logger.Debug("Extracted command from sh/bash -c wrapper: {Command}", command);
        }

        command = command.Trim();

        logger.Debug("Prepared command: {Command}", command);
        return command;
    }
}