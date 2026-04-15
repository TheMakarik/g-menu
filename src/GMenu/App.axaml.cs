using Avalonia.Threading;
using ILogger = Serilog.ILogger;

namespace GMenu;

public partial class App : Application
{
    public static IServiceProvider Services
    {
        get => field ?? throw new InvalidOperationException("Provider is unconfigured"); 
        set;
    }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

   
    public override async void OnFrameworkInitializationCompleted()
    {
        var provider = Services;
        provider.GetRequiredService<ILogger>().Information("Initializing GMenu...");
        LoadMaterialTheme(provider);
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow { DataContext = Services.GetRequiredService<MainWindowViewModel>() };
        

        await LoadConfigurationAsync(provider).ConfigureAwait(false);
        LoadLocalization(provider);
        
        base.OnFrameworkInitializationCompleted();
    }

    private async Task LoadConfigurationAsync(IServiceProvider provider)
    {
       await provider.GetRequiredService<IConfigurationProvider>().EnsureExistsAsync();
    }

    private void LoadMaterialTheme(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var gnomeLoader = scope.ServiceProvider.GetRequiredService<ILinuxThemeLoader>();
        var hexTheme = gnomeLoader.GetThemeHex();
        var materialTheme = this.LocateMaterialTheme<CustomMaterialTheme>();
        materialTheme.PrimaryColor = Color.Parse(hexTheme);
        materialTheme.SecondaryColor = Color.Parse(hexTheme);
        materialTheme.BaseTheme = BaseThemeMode.Inherit;
    }
    
    private void LoadLocalization(IServiceProvider provider)
    {
        var localizationProvider = provider.GetRequiredService<ILocalizationProvider>();
        localizationProvider.SetLocalization(new CultureInfo("ru-RU"));
    }
}