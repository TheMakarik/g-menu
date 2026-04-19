using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public partial class TreeViewModelDesktopFile : TreeViewModelBase
{
    private bool _searchingIcon = false;
    private readonly IDesktopFileIconPathRefiner _iconPathRefiner;
    private readonly string _iconPath;

    public string FilePath { get; }
    public string Name { get; }

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

   

    public TreeViewModelDesktopFile(string filePath, string iconPath, string name, 
        IDesktopFileIconPathRefiner iconPathRefiner, 
        ILogger logger,
        IRootRequirer rootRequirer,
        ILocalizationProvider localizationProvider) : base(logger, rootRequirer, localizationProvider)
    {
        FilePath = filePath;
        Name = name;
        _iconPathRefiner =  iconPathRefiner;
        _iconPath = iconPath;
        
    }
    

    private void BeginIconLoading()
    {
        _searchingIcon = true;
        _ = Task.Run(async () =>
        {
            var foundIcon = _iconPathRefiner.RefinePath(_iconPath);
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                IconPath = foundIcon;
            });
        });
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