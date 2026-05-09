using GMenu.Views.Utils;

namespace GMenu.Views;


public sealed partial class SelectFilesWindow : ReactiveWindow<SelectFilesWindowViewModel>
{
    public IReadOnlyCollection<string> Paths { get; }

    [DynamicDependency(DynamicallyAccessedMemberTypes.AllEvents, typeof(SelectFilesWindow))]
    public SelectFilesWindow(IReadOnlyCollection<string> paths)
    {
        Paths = paths;
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<SelectFilesWindowViewModel>();
        ViewModel!.OpenDirectory.RegisterHandler(async context =>
        {
            context.SetOutput(Unit.Default);
            var topLevel = GetTopLevel(this);
            Debug.Assert(topLevel is not null);

            if (!Directory.Exists(context.Input))
            {
                await MessageBoxUtil.ShowCannotFindDirectoryBoxAsync(context.Input, ViewModel!.LocalizationProvider, this);
                return;
            }
            
            var success = await topLevel.Launcher.LaunchDirectoryInfoAsync(new DirectoryInfo(context.Input));
            if (!success)
            {
                await MessageBoxUtil.ShowCannotFindDirectoryBoxAsync(context.Input, ViewModel!.LocalizationProvider, this);
            }
        });
    }
    
}