using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using GMenu.Services.LinuxSystem.interfaces;
using GMenu.ViewModels;
using GMenu.Views;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using Microsoft.Extensions.DependencyInjection;

namespace GMenu;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var _provider = new Bootstrapper().BuildApplication();
        LoadMaterialTheme(_provider);
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void LoadMaterialTheme(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var gnomeLoader = scope.ServiceProvider.GetRequiredService<IGNOMEThemeLoader>();
        var hexTheme = gnomeLoader.GetThemeHex();
        var materialTheme = this.LocateMaterialTheme<CustomMaterialTheme>();
        materialTheme.PrimaryColor = Color.Parse(hexTheme);
        materialTheme.SecondaryColor = Color.Parse(hexTheme);
        materialTheme.BaseTheme = BaseThemeMode.Inherit;
    }
}