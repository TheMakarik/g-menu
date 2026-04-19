using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public abstract partial class TreeViewModelBase(ILogger logger, IRootRequirer rootRequirer, ILocalizationProvider localizationProvider) : ViewModelBase(logger, rootRequirer, localizationProvider)
{
    public  required TreeViewModelBase?  Parent { get; init; }
    [Reactive] private ObservableCollection<TreeViewModelBase> _children = [];
    
    public bool IsSelected
    {
        get => field;
        set
        {
            if (value)
                SendSelectionChangeMessage();
            this.RaiseAndSetIfChanged(ref field, value);
        }
    }

    
    public abstract void SendSelectionChangeMessage();
    
}