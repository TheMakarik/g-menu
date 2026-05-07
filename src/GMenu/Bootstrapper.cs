
namespace GMenu;

public sealed class Bootstrapper
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(RollingInterval))]
    public IServiceProvider BuildApplication()
    {
        GMenuOptions? options;
        
        using (var stream = File.Open(StaticConfiguration.ConfigurationPath, FileMode.Open))
#pragma warning disable IL2026
#pragma warning disable IL3050
            options = JsonSerializer.Deserialize<GMenuOptions?>(stream, GMenuOptionsSerializationContext.Default.Options);
#pragma warning restore IL2026
#pragma warning restore IL3050
        
        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.WithThreadId()
            .WriteTo.File(
                outputTemplate: options!.Logging.OutputTemplate,
                rollingInterval: Enum.Parse<RollingInterval>(options.Logging.RollingInterval),
                path: Path.Join(
                    options.Configuration.Directory, 
                    options.Logging.LogsDirectory, 
                    options.Logging.LogFileNamePrefix))
                    .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: options.Logging.OutputTemplate);
        
        
        Log.Logger = loggerConfiguration.CreateLogger();
        
        var services = new ServiceCollection();

        
        Log.Logger.Information("Configure services...");
        services
            .AddSingleton(options)
            .AddSingleton<IDesktopFileIconPathRefiner, DesktopFileIconPathRefiner>()
            .AddSingleton<IDesktopFileReader, DesktopFileReader>()
            .AddSingleton<IDesktopFileHeaderReader, DesktopFileHeaderReader>()
            .AddSingleton<DesktopFilesTreeViewModel>()
            .AddSingleton<MainWindowViewModel>()
            .AddTransient<InfoWindowViewModel>()
            .AddTransient<SelectFilesWindowViewModel>()
            .AddSingleton<IDesktopFilesHeaderSearcher, DesktopFileHeaderSearcher>()
            .AddSingleton<IDesktopFilesExecFormatter, DesktopFilesExecFormatter>()
            .AddSingleton(Log.Logger)
            .AddSingleton<IConfigurationProvider, ConfigurationProvider>()
            .AddSingleton<ILinuxTerminalLauncher, LinuxTerminalLauncher>()
            .AddSingleton<IDesktopFilesRunner, DesktopFilesRunner>()
            .AddScoped<ILinuxThemeLoader, LinuxThemeLoader>()
            .AddSingleton<ILocalizationProvider, JsonLocalizationProvider>();
        
        return services.BuildServiceProvider();
        
    }
}