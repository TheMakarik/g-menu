using GMenu.Modules.ColorUtils.Interfaces;

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
        try
        {
            var provider = Services;
            provider.GetRequiredService<ILogger>().Information("Initializing GMenu...");
            await LoadMaterialThemeAsync(provider);
            provider.GetRequiredService<ILogger>().Information("Desktop files paths: {paths}", StaticConfiguration.PathToDesktopFiles);
            provider.GetRequiredService<ILogger>().Information("Desktop files icons path: {paths}", StaticConfiguration.PathsToRefineIcon);
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new MainWindow { DataContext = Services.GetRequiredService<MainWindowViewModel>() };
        
            var configurationTask =  LoadConfigurationAsync(provider);
            var localizationTask = LoadLocalizationAsync(provider);
            
            await Task.WhenAll(configurationTask, localizationTask).ConfigureAwait(false);
        
            base.OnFrameworkInitializationCompleted();
        }
        catch (Exception e)
        {
            Services.GetRequiredService<ILogger>().Error(e, "An error occured while initializing the application.");
            Console.WriteLine(e); 
            Environment.Exit(-1);
        }
    }

    private async Task LoadConfigurationAsync(IServiceProvider provider)
    {
       await provider.GetRequiredService<IConfigurationProvider>().EnsureExistsAsync();
    }

    private async Task LoadMaterialThemeAsync(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
    
        var themeLoader = scope.ServiceProvider.GetRequiredService<ILinuxThemeLoader>();
        var rgb = await themeLoader.GetThemeHexAsync();
        
        if (rgb is null)
        {
            Environment.Exit(-1);
        }
        
        var color = scope.ServiceProvider.GetRequiredService<INearСolorValueSearch>().GetNearColor(rgb);
        
     
        
        var materialTheme = this.LocateMaterialTheme<MaterialTheme>();
        materialTheme.BaseTheme = BaseThemeMode.Inherit;
        materialTheme.PrimaryColor = color.GetMaterialPrimaryColor() ?? default;
        materialTheme.SecondaryColor = color.GetMaterialSecondaryColor() ?? default;
        
        this.Resources["AccentColor"] = materialTheme.PrimaryColor;
    }
    
    private async Task LoadLocalizationAsync(IServiceProvider provider)
    {
       var localizationProvider = provider.GetRequiredService<ILocalizationProvider>();
       await localizationProvider.SetLocalizationAsync(new CultureInfo("ru-RU"));
    }
}