namespace GMenu;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configurationProvider, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
    {
        var provider = Design.IsDesignMode
            ? new ServiceCollection().BuildServiceProvider() 
            : new Bootstrapper().BuildApplication();

        if (!Design.IsDesignMode)
        {
            Locator.CurrentMutable.UseSerilogFullLogger(Log.Logger);
        }
        
        App.Services =  provider;
        
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI(builder => builder
                .BuildApp());
    }
     
}