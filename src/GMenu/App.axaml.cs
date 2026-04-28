
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
            //https://stackoverflow.com/questions/79933121/avaloniaui-strange-async-application-loading
            var provider = Services;
            LoadLocalization(provider);
            provider.GetRequiredService<ILogger>().Information("Initializing GMenu...");
            //await LoadMaterialThemeAsync(provider);
            provider.GetRequiredService<ILogger>().Information("Desktop files paths: {paths}", StaticConfiguration.PathToDesktopFiles);
            provider.GetRequiredService<ILogger>().Information("Desktop files icons path: {paths}", StaticConfiguration.PathsToRefineIcon);
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new MainWindow { DataContext = Services.GetRequiredService<MainWindowViewModel>() };
            
            await LoadConfigurationAsync(provider).ConfigureAwait(false);
          
        
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
        
        var materialTheme = this.LocateMaterialTheme<CustomMaterialTheme>();
        materialTheme.BaseTheme = BaseThemeMode.Inherit;
        materialTheme.PrimaryColor = new Color(255, (byte)rgb.Red, (byte)rgb.Green, (byte)rgb.Blue );
        materialTheme.SecondaryColor = new Color(255, (byte)rgb.Red, (byte)rgb.Green, (byte)rgb.Blue );
        
        this.Resources["AccentColor"] = materialTheme.PrimaryColor;
    }
    
    private void LoadLocalization(IServiceProvider provider)
    {
       var localizationProvider = provider.GetRequiredService<ILocalizationProvider>();
       localizationProvider.SetLocalization(new CultureInfo("ru-RU"));
    }
}