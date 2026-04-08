using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GMenu.Models.Common;

using GMenu.Models.GNOME;
using GMenu.Services.LinuxSystem.interfaces;
using Serilog;

namespace GMenu.Services.LinuxSystem;

public sealed class GNOMEThemeLoader(ILogger logger) : IGNOMEThemeLoader
{
    private const string GioLibrary = "libgio-2.0.so.0";
    private const string GNOMEScheme = "org.gnome.desktop.interface";
    private const string AccentColorKey = "accent-color";
    
    [DllImport(GioLibrary, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr g_settings_new(string schema);

    [DllImport(GioLibrary, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr g_settings_get_string(IntPtr settings, string key);

    [DllImport(GioLibrary, CallingConvention = CallingConvention.Cdecl)]
    private static extern void g_object_unref(IntPtr obj);

    [DllImport(GioLibrary, CallingConvention = CallingConvention.Cdecl)]
    private static extern void g_free(IntPtr mem);
    
    public string GetThemeHex()
    {
        var settingsPointer = IntPtr.Zero;
        var accentColorNamePointer = IntPtr.Zero;
        
        
        try
        {
            settingsPointer = g_settings_new(GNOMEScheme);
            accentColorNamePointer = g_settings_get_string(settingsPointer, AccentColorKey);
            
            var accentColorName = (Marshal.PtrToStringUTF8(accentColorNamePointer) ?? string.Empty)
                .Replace("`", string.Empty)
                .Replace("'", string.Empty);
            
            logger.Information("GNOME accent color loaded: {accentColorName}", accentColorName);

            var accentHex = StaticConfiguration.AccentColorMap.TryGetValue(accentColorName, out var result)
                ? result
                : StaticConfiguration.AccentColorMap[StaticConfiguration.DefaultAccentColor];
            
            logger.Debug("GNOME accent hex loaded: {accentHex}", accentHex);
            return accentHex;
        }
        finally
        {
            if (accentColorNamePointer != IntPtr.Zero)
                g_free(accentColorNamePointer);
            if (settingsPointer != IntPtr.Zero)
                g_object_unref(settingsPointer);
        }
    }
}