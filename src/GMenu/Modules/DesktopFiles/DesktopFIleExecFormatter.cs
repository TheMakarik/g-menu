namespace GMenu.Modules.DesktopFiles;

public sealed partial class DesktopFilesExecFormatter : IDesktopFilesExecFormatter
{
    [GeneratedRegex(@"\b(DISABLE_\w+=\d+)\s+")]
    private static partial Regex DisableEnvRegex();
    
    [GeneratedRegex(@"\b(SNAP_\w+=\S+)\s+")]
    private static partial Regex SnapEnvRegex();
    
    [GeneratedRegex(@"\b(desktop-launch|snap_wrapper)\s+")]
    private static partial Regex SnapWrapperRegex();

    public string PrepareCommand(ReadOnlySpan<char> headerExec)
    {
        var command = RemoveFieldCodes(headerExec);
        command = UnwrapShCommand(command.AsSpan());
        command = RemoveFlatpakMarkers(command.AsSpan());
        command = FixSnapCommand(command.AsSpan());
        command = CleanWhitespace(command);
        return command;
    }

    public IReadOnlyList<string> ParseCommandLine(string commandLine)
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

    public string EscapeForShSingleQuotes(string argument)
    {
        if (string.IsNullOrEmpty(argument))
            return "\"\"";
        
        var escaped = argument
            .Replace("\\", "\\\\")  
            .Replace("\"", "\\\"")  
            .Replace("$", "\\$")  
            .Replace("`", "\\`");  

        return $"'{escaped}'";
    }

    private static string RemoveFieldCodes(ReadOnlySpan<char> exec)
    {
        var result = new StringBuilder(exec.Length);
        for (var i = 0; i < exec.Length; i++)
        {
            if (exec[i] == '%' && i + 1 < exec.Length)
            {
                var code = exec[i + 1];
                if (IsFieldCode(code))
                {
                    i++;
                    continue;
                }
                if (code == '{' && i + 2 < exec.Length)
                {
                    var closeBrace = exec[i + 2];
                    if (IsFieldCode(closeBrace))
                    {
                        i += 2;
                        continue;
                    }
                }
            }
            result.Append(exec[i]);
        }
        return result.ToString();
    }

    private static bool IsFieldCode(char code) => code switch
    {
        'f' or 'F' or 'u' or 'U' or 'd' or 'D' or
        'n' or 'N' or 'i' or 'c' or 'k' or 'v' or 'm' => true,
        _ => false
    };

    private static string UnwrapShCommand(ReadOnlySpan<char> command)
    {
        var trimmed = command.Trim();
        if (!trimmed.StartsWith("sh -c ".AsSpan(), StringComparison.OrdinalIgnoreCase) &&
            !trimmed.StartsWith("bash -c ".AsSpan(), StringComparison.OrdinalIgnoreCase))
            return command.ToString();

        var dashCIndex = trimmed.IndexOf("-c".AsSpan(), StringComparison.Ordinal);
        if (dashCIndex == -1) return command.ToString();

        var afterC = trimmed.Slice(dashCIndex + 2).TrimStart();

        if (afterC.Length > 0 && afterC[0] == '"')
        {
            var endQuote = afterC.Slice(1).IndexOf('"');
            if (endQuote > 0) return afterC.Slice(1, endQuote).ToString();
        }
        if (afterC.Length > 0 && afterC[0] == '\'')
        {
            var endQuote = afterC.Slice(1).IndexOf('\'');
            if (endQuote > 0) return afterC.Slice(1, endQuote).ToString();
        }
        return command.ToString();
    }

    private static string RemoveFlatpakMarkers(ReadOnlySpan<char> command)
    {
        var result = new StringBuilder(command.Length);
        var i = 0;
        while (i < command.Length)
        {
            if (command[i] == '@' && i + 1 < command.Length && command[i + 1] == '@')
            {
                i += 2;
                if (i < command.Length && (command[i] == 'u' || command[i] == 'U' || command[i] == 'f' || command[i] == 'F'))
                    i++;
                while (i < command.Length && char.IsWhiteSpace(command[i]))
                    i++;
                continue;
            }
            result.Append(command[i]);
            i++;
        }
        return result.ToString();
    }

    private static string FixSnapCommand(ReadOnlySpan<char> command)
    {
        var result = command.ToString();
        if (!result.Contains("/snap/bin/") && !result.Contains("snap run"))
            return result;
        result = DisableEnvRegex().Replace(result, "");
        result = SnapEnvRegex().Replace(result, "");
        result = SnapWrapperRegex().Replace(result, "");
        return result.Trim();
    }

    private static string CleanWhitespace(string command)
    {
        var parts = command.Split(default(char[]), StringSplitOptions.RemoveEmptyEntries);
        return string.Join(" ", parts);
    }
}