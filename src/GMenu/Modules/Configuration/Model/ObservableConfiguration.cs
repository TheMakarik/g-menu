namespace GMenu.Modules.Configuration.Model;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public sealed class ObservableConfiguration : INotifyPropertyChanged
{
    private bool _isObserving = false;
    public event PropertyChangedEventHandler? PropertyChanged;

    
    public ObservableCollection<CustomUserCategory>? CustomCategories
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

    public bool LocalizeDesktopFiles
    {
        get;
        set => SetField(ref field, value);
    }

    public bool ShowCannotLoadThemeFromDBusMessage
    {
        get;
        set =>  SetField(ref field, value);
    }
    
    public required Version Version { get; set; }
    
    
    public void BeginPropertyChangeRaising()
    {
        _isObserving = true;

        var propertiesToRedirectEvents = this.GetType()
            .GetProperties()
            .Select(property => new { PropertyInfo = property, Value = property.GetValue(this) })
            .Where(property => property.Value is INotifyCollectionChanged)
            .Select(property => (INotifyCollectionChanged?)property.Value);
        
        foreach (var property in propertiesToRedirectEvents)
            property?.CollectionChanged += (sender, args) =>
            {
                if (!_isObserving)
                    return;
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Reset:
                        OnPropertyChanged(sender!.GetType().Name);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        PropertyChanged?.Invoke(this, FormatEventArgs(sender, $"{args.OldStartingIndex}...{args.NewStartingIndex}"));
                        break;
                    default:
                        PropertyChanged?.Invoke(this, FormatEventArgs(sender, args.NewStartingIndex.ToString()));
                        break;
                }
            };
    }
    

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) 
            return;
        field = value;
        if(_isObserving)
              OnPropertyChanged(propertyName);
    }
    
    private PropertyChangedEventArgs FormatEventArgs(object? sender, string index)
    {
        ArgumentNullException.ThrowIfNull(sender);
        return new PropertyChangedEventArgs($"{sender!.GetType().Name}[{index}]");
    }
}