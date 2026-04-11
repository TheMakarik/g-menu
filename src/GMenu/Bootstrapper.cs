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