using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public partial class TreeViewModelBase(ILogger logger, IRootRequirer rootRequirer, ILocalizationProvider localizationProvider) : ViewModelBase(logger, rootRequirer, localizationProvider)
{
    protected Func<object>? MessageToSendOnTrueSelectedUpdateFunction { get; set; }= null;
    public bool IsSelected
    {
        get => field;
        set
        {
            if(value && MessageToSendOnTrueSelectedUpdateFunction is not null)
                MessageBus.Current.SendMessage(MessageToSendOnTrueSelectedUpdateFunction());
            this.RaiseAndSetIfChanged(ref field, value);
        }
    }

    [Reactive] private ObservableCollection<TreeViewModelBase> _children = [];
}