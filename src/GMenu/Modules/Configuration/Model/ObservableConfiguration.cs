namespace GMenu.Modules.Configuration.Model;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public sealed class ObservableConfiguration : INotifyPropertyChanged
{
    private bool _isObserving = false;
    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableConfiguration()
    {
        var propertiesToRedirectEvents = this.GetType()
            .GetProperties()
            .Where(propertyInfo => propertyInfo.PropertyType == typeof(INotifyCollectionChanged))
            .Select(propertyInfo => (INotifyCollectionChanged)propertyInfo.GetValue(this)!);

        foreach (var propertyInfo in propertiesToRedirectEvents)
            propertyInfo.CollectionChanged += (sender, args) =>
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Reset:
                        OnPropertyChanged(sender!.GetType().Name);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        PropertyChanged?.Invoke(this, FormatEventArgs(sender, args.NewStartingIndex));
                        PropertyChanged?.Invoke(this, FormatEventArgs(sender, args.OldStartingIndex));
                        break;
                    default:
                        PropertyChanged?.Invoke(this, FormatEventArgs(sender, args.NewStartingIndex));
                        break;
                }
            };
    }
    
    
    public ObservableCollection<UnexistingCategory> UnexistingCategories
    {
        get;
        set => SetField(ref field, value);
    }

    public required CultureInfo Language
    {
        get;
        set => SetField(ref field, value);
    }
    
    public string? AccentColor
    {
        get;
        set => SetField(ref field, value);
    }
    
    public BaseTheme? Theme
    { 
        get;
        set => SetField(ref field, value);
    }

    public bool LocalizeDesktopFileNames
    {
        get;
        set => SetField(ref field, value);
    }

    public void BeginPropertyChangeRaising()
    {
        _isObserving = true; 
    }
    

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) 
            return false;
        field = value;
        if(_isObserving)
              OnPropertyChanged(propertyName);
        return true;
    }
    
    private PropertyChangedEventArgs FormatEventArgs(object? sender, int index)
    {
        ArgumentNullException.ThrowIfNull(sender);
        return new PropertyChangedEventArgs($"{sender!.GetType().Name}[{index}]");
    }
}