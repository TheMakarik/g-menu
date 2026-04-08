using System;
using GMenu.Services.LinuxSystem;
using GMenu.Services.LinuxSystem.interfaces;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

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
            .CreateLogger();
        
        var services = new ServiceCollection();

        services
            .AddSingleton(Log.Logger)
            .AddScoped<IGNOMEThemeLoader, GNOMEThemeLoader>()
            .AddSingleton<IRootRequirer, RootRequirer>();
        
        
        return services.BuildServiceProvider();
        
    }
}