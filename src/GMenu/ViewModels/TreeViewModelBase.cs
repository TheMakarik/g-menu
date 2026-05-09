namespace GMenu.ViewModels;

public abstract partial class TreeViewModelBase(ILogger logger, ILocalizationProvider localizationProvider) : ViewModelBase(localizationProvider)
{
    public TreeViewModelBase? Parent { get; init; }
    [Reactive] private ObservableCollection<TreeViewModelBase> _children = [];
    
    public bool IsSelected
    {
        get => field;
        set
        {
            if (value)
                MessageBus.Current.SendMessage(new SendSelectedItemMessage(){SelectedItem = this});
            this.RaiseAndSetIfChanged(ref field, value);
        }
    }

    [Obsolete("UI do not use this API Anymore, so you can delete it")]
    public abstract void SendSelectionChangeMessage();
    
}