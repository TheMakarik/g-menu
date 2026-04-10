using System;
using System.Text.Json;
using GMenu.Modules.LinuxSystem;
using GMenu.Modules.LinuxSystem.interfaces;
using GMenu.Modules.Localization;
using GMenu.Modules.Localization.Interfaces;
using GMenu.Views.Utils;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using static Avalonia.Application;

namespace GMenu;

public sealed class Bootstrapper
{
    public IServiceProvider BuildApplication()
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithThreadId()
#if  DEBUG
            .WriteTo.Console(
                theme: AnsiConsoleTheme.Code,
                outputTemplate: StaticConfiguration.SerilogOuputTemplate)
#endif
            .MinimumLevel.Debug()
            .CreateLogger();
        
        var services = new ServiceCollection();

        services
            .AddSingleton(Log.Logger)
            .AddScoped<IGNOMEThemeLoader, GNOMEThemeLoader>()
            .AddSingleton<IRootRequirer, RootRequirer>();
        
        services.AddSingleton<ILocalizationProvider>(new DynamicLocalizationProvider(
            updateUIOnCultureChanging: (@new, old) => ResourceDictionaryUtil.Replace(old.ToString(), @new.ToString()),
            getStringDynamic: (key) => Current!.Resources[key.ToString()]?.ToString() ?? key.ToString()
        ));;
        
        return services.BuildServiceProvider();
        
    }
}