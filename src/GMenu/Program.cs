using Splat;
using Splat.Serilog;

namespace GMenu;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        var provider = Design.IsDesignMode
            ? new ServiceCollection().BuildServiceProvider() 
            : new Bootstrapper().BuildApplication();

        if (!Design.IsDesignMode)
        {
            provider.UseMicrosoftDependencyResolver();
            Locator.CurrentMutable.UseSerilogFullLogger(Log.Logger);
        }
        
        App.Services =  provider;
        
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI(builder => builder.BuildApp());
    }
     
}