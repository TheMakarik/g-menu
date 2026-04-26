namespace GMenu;

public sealed class Bootstrapper
{
    public IServiceProvider BuildApplication()
    {
        GMenuOptions options;
        
        using (var stream = File.Open(StaticConfiguration.ConfigurationPath, FileMode.Open))
#pragma warning disable IL2026
#pragma warning disable IL3050
            options = JsonSerializer.Deserialize<GMenuOptions>(stream, GMenuOptionsSerializationContext.Default.Options);
#pragma warning restore IL2026
#pragma warning restore IL3050
        
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithThreadId()
#if DEBUG
            .WriteTo.Console(
                theme: options!.Logging.SerilogConsoleTheme,
                outputTemplate: options.Logging.OutputTemplate)
#endif
            .WriteTo.File(
                outputTemplate: options.Logging.OutputTemplate,
                rollingInterval: options.Logging.RollingInterval,
                path: Path.Combine(
                    options.Configuration.Directory, 
                    options.Logging.LogsDirectory, 
                    options.Logging.LogFileNamePrefix))
            .MinimumLevel.Debug()
            .CreateLogger();
        
        var services = new ServiceCollection();

        services
            .AddSingleton(options)
            .AddSingleton<IDesktopFileIconPathRefiner, DesktopFileIconPathRefiner>()
            .AddSingleton<IDesktopFileHeaderReader, DesktopFileHeaderReader>()
            .AddSingleton<DesktopFilesTreeViewModel>()
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton(Log.Logger)
            .AddSingleton<IConfigurationProvider, ConfigurationProvider>()
            .AddScoped<ILinuxThemeLoader, LinuxThemeLoader>()
            .AddSingleton<ILocalizationProvider, JsonLocalizationProvider>()
            .AddSingleton<IRootRequirer, RootRequirer>();
        
        return services.BuildServiceProvider();
        
    }
}