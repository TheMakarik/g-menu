namespace GMenu.ViewModels;

public partial class TreeViewModelDesktopFile(
    DesktopFileHeader desktopFileHeader,
    IDesktopFilesRunner desktopFilesRunner,
    IDesktopFileIconPathRefiner iconPathRefiner,
    ILogger logger,
    ILocalizationProvider localizationProvider)
    : TreeViewModelBase(logger, localizationProvider)
{
    private bool _searchingIcon = false;

    public string FilePath { get; } = desktopFileHeader.Path;
    public string? Name { get; } = desktopFileHeader.Name;
    public DesktopFileHeader Header => desktopFileHeader;
    
    public string? IconPath
    {
        get
        {
            if (MustLoadIcon(field))
                BeginIconLoading();
            return field;
        }
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
    [ReactiveCommand]
    private void RunDesktopFile()
    {
        Task.Factory.StartNew(async void () =>
        {
            await desktopFilesRunner.RunDesktopFileFromHeaderAsync(
                desktopFileHeader,
                requireSudo: false,
                new CancellationTokenSource());
        }, TaskCreationOptions.LongRunning);
    }
    
    [ReactiveCommand]
    private void SudoRunDesktopFile()
    {
        Task.Factory.StartNew(async void () =>
        {
            await desktopFilesRunner.RunDesktopFileFromHeaderAsync(
               desktopFileHeader,
                requireSudo: true,
                new CancellationTokenSource());
        }, TaskCreationOptions.LongRunning);
    }
    

    private void BeginIconLoading()
    {
        _searchingIcon = true;
        Observable.Start(() => iconPathRefiner.RefinePath(desktopFileHeader.IconPath, StaticConfiguration.PathsToRefineIcon))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(result => IconPath = result);
    }
    
    private bool MustLoadIcon(string? iconPathField)
    {
        return iconPathField is null && !_searchingIcon;
    }

    public override void SendSelectionChangeMessage()
    {
        this.Parent!.SendSelectionChangeMessage();
        var message = new UpdateSelectedDesktopFileMessage()
        {
            Name = Name ?? string.Empty,
        };
        MessageBus.Current.SendMessage(message);
    }
}