using static Avalonia.Application;
using ILogger = Serilog.ILogger;

namespace GMenu;

public sealed class Bootstrapper
{
    public IServiceProvider BuildApplication()
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithThreadId()
            .WriteTo.Console(
                theme: StaticConfiguration.SerilogConsoleTheme,
                outputTemplate: StaticConfiguration.SerilogOutputTemplate)
            .WriteTo.File(
                outputTemplate: StaticConfiguration.SerilogOutputTemplate,
                rollingInterval: StaticConfiguration.SerilogRollingInterval,
                path: StaticConfiguration.LogsPath)
            .MinimumLevel.Debug()
            .CreateLogger();
        
        var services = new ServiceCollection();

        services
            .AddSingleton<IDesktopFileIconPathRefiner, DesktopFileIconPathRefiner>()
            .AddSingleton<IDesktopFileHeaderReader, DesktopFileHeaderReader>()
            .AddSingleton<DesktopFilesTreeViewModel>()
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton(Log.Logger)
            .AddSingleton<IConfigurationProvider, ConfigurationProvider>()
            .AddScoped<ILinuxThemeLoader, LinuxThemeLoader>()
            .AddSingleton<IRootRequirer, RootRequirer>();

        services.AddSingleton<ILocalizationProvider>(provider => new DynamicLocalizationProvider(
                updateUIOnCultureChanging: (@new) => Dispatcher.UIThread.Invoke(()
                    => Resources.Resources.Culture = @new),
                getStringDynamic: (key, culture) => Resources.Resources.ResourceManager.GetString(key, culture) ?? StaticConfiguration.CannotFoundKeyInLocalizationValue, 
                provider.GetRequiredService<ILogger>()));
        
        return services.BuildServiceProvider();
        
    }
}