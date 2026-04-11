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
                theme: StaticConfiguration.SerilogConsoleTheme,
                outputTemplate: StaticConfiguration.SerilogOutputTemplate)
#endif
            .WriteTo.File(
                outputTemplate: StaticConfiguration.SerilogOutputTemplate,
                rollingInterval: StaticConfiguration.SerilogRollingInterval,
                path: StaticConfiguration.LogsPath)
            .MinimumLevel.Debug()
            .CreateLogger();
        
        var services = new ServiceCollection();

        services
            .AddSingleton(Log.Logger)
            .AddSingleton<IConfiguration, Configuration>()
            .AddScoped<ILinuxThemeLoader, LinuxThemeLoader>()
            .AddSingleton<IRootRequirer, RootRequirer>();
        
        services.AddSingleton<ILocalizationProvider>(new DynamicLocalizationProvider(
            updateUIOnCultureChanging: (@new, old) => ResourceDictionaryUtil.Replace(old.ToString(), @new.ToString()),
            getStringDynamic: (key) => Current!.Resources[key.ToString()]?.ToString() ?? key.ToString()
        ));;
        
        return services.BuildServiceProvider();
        
    }
}