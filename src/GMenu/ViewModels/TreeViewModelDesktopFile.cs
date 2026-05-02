namespace GMenu.ViewModels;

public partial class TreeViewModelDesktopFile(
    string filePath,
    string iconPath,
    string name,
    IDesktopFileIconPathRefiner iconPathRefiner,
    ILogger logger,
    IRootRequirer rootRequirer,
    ILocalizationProvider localizationProvider)
    : TreeViewModelBase(logger, rootRequirer, localizationProvider)
{
    private bool _searchingIcon = false;

    public string FilePath { get; } = filePath;
    public string Name { get; } = name;

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
    private async Task RunDesktopFile()
    {
    }
    

    private void BeginIconLoading()
    {
        _searchingIcon = true;
        Observable.Start(() => iconPathRefiner.RefinePath(iconPath, StaticConfiguration.PathsToRefineIcon))
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
            Name = Name,
        };
        MessageBus.Current.SendMessage(message);
    }
}