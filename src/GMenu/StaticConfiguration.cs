using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;

namespace GMenu;

public static class StaticConfiguration
{
    public static readonly string ConfigurationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "g-menu");
    public const string DefaultAccentColor = "blue";
    public const string SerilogOuputTemplate = "[{Level:u3}] [{Timestamp:yyyy-MM-dd HH:mm}] [{ThreadId}] {Message:lj}{NewLine}{Exception}";
    
    public static readonly FrozenDictionary<string, string> AccentColorMap = new Dictionary<string, string>()
    {
        ["blue"]   = "#3584E4",
        ["teal"]   = "#2190A4",
        ["green"]  = "#3A9446",
        ["yellow"] = "#C88800",
        ["orange"] = "#ED7B2B",
        ["red"]    = "#E62D42",
        ["pink"]   = "#D56199",
        ["purple"] = "#9141AC",
        ["slate"]  = "#6F8396"
    }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
}