namespace GMenu.Modules.Configuration.Model;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public sealed class ObservableConfiguration : INotifyPropertyChanged
{
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


    public required User User
    {
        get;
        set => SetField(ref field, value);
    }

    public required ObservableCollection<DesktopFileDirectory> SearchDesktopFilesDirectories
    {
        get;
        set => SetField(ref field, value);
    }

    public required ObservableCollection<UnexistingCategory> UnexistingCategories
    {
        get;
        set => SetField(ref field, value);
    }


    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) 
            return false;
        var mustRaisePropertyChanged = field is not null;
        field = value;
        if(mustRaisePropertyChanged)
            OnPropertyChanged(propertyName);
        return true;
    }
    
    private PropertyChangedEventArgs FormatEventArgs(object? sender, int index)
    {
        ArgumentNullException.ThrowIfNull(sender);
        return new PropertyChangedEventArgs($"{sender!.GetType().Name}[{index}]");
    }
}