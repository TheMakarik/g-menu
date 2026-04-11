namespace GMenu;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        var provider = new Bootstrapper().BuildApplication();
        LoadMaterialTheme(provider);
        LoadLocalization(provider);
        await LoadConfigurationAsync(provider);
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async Task LoadConfigurationAsync(IServiceProvider provider)
    {
        await provider.GetRequiredService<IConfiguration>().EnsureExistsAsync();
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