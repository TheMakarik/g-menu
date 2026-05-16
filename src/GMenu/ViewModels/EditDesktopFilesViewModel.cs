namespace GMenu.ViewModels;

public sealed class EditDesktopFilesViewModel : ViewModelBase
{
    private DesktopFile? _desktopFile;
    private readonly IDesktopFilePropertiesUpdater _propertiesUpdater;

    public string? IconPath
    {
        get => _desktopFile?.IconPath;
        set => UpdateDesktopFileProperty(nameof(IconPath), DesktopFileKeys.IconKey, value);
    }
    
    public string? Exec
    {
        get => _desktopFile?.Exec;
        set => UpdateDesktopFileProperty(nameof(Exec), DesktopFileKeys.ExecKey, value);
    }
    
    public string? Name
    {
        get => _desktopFile?.Name;
        set => UpdateDesktopFileProperty(nameof(Name), _desktopFile?.NameKey ?? DesktopFileKeys.NameKey, value);
    }
    
    public string? Comment
    {
        get => _desktopFile?.Comment;
        set => UpdateDesktopFileProperty(nameof(Comment), _desktopFile?.CommentKey ?? DesktopFileKeys.CommentKey, value);
    }
    
    public string? GenericName
    {
        get => _desktopFile?.GenericName;
        set => UpdateDesktopFileProperty(nameof(GenericName), _desktopFile?.GenericNameKey ?? DesktopFileKeys.GenericNameKey, value);
    }
    
    public string? Keywords
    {
        get => _desktopFile?.Keywords;
        set => UpdateDesktopFileProperty(nameof(Keywords), _desktopFile?.KeywordsKey ?? DesktopFileKeys.KeywordsKey, value);
    }
    
    public string? Categories
    {
        get => _desktopFile?.Categories;
        set => UpdateDesktopFileProperty(nameof(Categories), DesktopFileKeys.CategoriesKey, value);
    }
    
    public bool? NoDisplay
    {
        get => _desktopFile?.NoDisplay;
        set => UpdateDesktopFileProperty(nameof(NoDisplay), DesktopFileKeys.NoDisplayKey, value);
    }
    
    public bool? Terminal
    {
        get => _desktopFile?.Terminal;
        set => UpdateDesktopFileProperty(nameof(Terminal), DesktopFileKeys.TerminalKey, value);
    }
    
    public bool? Hidden
    {
        get => _desktopFile?.Hidden;
        set => UpdateDesktopFileProperty(nameof(Hidden), DesktopFileKeys.HiddenKey, value);
    }
    
    public bool? StartupNotify
    {
        get => _desktopFile?.StartupNotify;
        set => UpdateDesktopFileProperty(nameof(StartupNotify), DesktopFileKeys.StartupNotifyKey, value);
    }
    
    public string? StartupWmClass
    {
        get => _desktopFile?.StartupWmClass;
        set => UpdateDesktopFileProperty(nameof(StartupWmClass), DesktopFileKeys.StartupWmClassKey, value);
    }
    
    public string? MimeTypes
    {
        get => _desktopFile?.MimeTypes;
        set => UpdateDesktopFileProperty(nameof(MimeTypes), DesktopFileKeys.MimeTypeKey, value);
    }
    
    public bool? SingleMainWindow
    {
        get => _desktopFile?.SingleMainWindow;
        set => UpdateDesktopFileProperty(nameof(SingleMainWindow), DesktopFileKeys.SingleMainWindowKey, value);
    }
    
    public string? Implements
    {
        get => _desktopFile?.Implements;
        set => UpdateDesktopFileProperty(nameof(Implements), DesktopFileKeys.ImplementsKey, value);
    }
    
    public string? TryExec
    {
        get => _desktopFile?.TryExec;
        set => UpdateDesktopFileProperty(nameof(TryExec), DesktopFileKeys.TryExecKey, value);
    }
    
    public bool? DBusActivatable
    {
        get => _desktopFile?.DBusActivatable;
        set => UpdateDesktopFileProperty(nameof(DBusActivatable), DesktopFileKeys.DBusActivatableKey, value);
    }
    
    public string? OnlyShowIn
    {
        get => _desktopFile?.OnlyShowIn;
        set => UpdateDesktopFileProperty(nameof(OnlyShowIn), DesktopFileKeys.OnlyShowInKey, value);
    }
    
    public string? NotShowIn
    {
        get => _desktopFile?.NotShowIn;
        set => UpdateDesktopFileProperty(nameof(NotShowIn), DesktopFileKeys.NotShowInKey, value);
    }
    
    public bool? XGnomeAutoRestart
    {
        get => _desktopFile?.XGnomeAutoRestart;
        set => UpdateDesktopFileProperty(nameof(XGnomeAutoRestart), DesktopFileKeys.XGnomeAutoRestartKey, value);
    }
    
    public string? XGnomeUsesNotifications
    {
        get => _desktopFile?.XGnomeUsesNotifications;
        set => UpdateDesktopFileProperty(nameof(XGnomeUsesNotifications), DesktopFileKeys.XGnomeUsesNotificationsKey, value);
    }
    
    public string? WorkingDirectory
    {
        get => _desktopFile?.WorkingDirectory;
        set => UpdateDesktopFileProperty(nameof(WorkingDirectory), DesktopFileKeys.PathKey, value);
    }
    
    public string? Version
    {
        get => _desktopFile?.Version;
        set => UpdateDesktopFileProperty(nameof(Version), DesktopFileKeys.VersionKey, value);
    }
    
    public EditDesktopFilesViewModel(ILocalizationProvider localizationProvider, IDesktopFileEntityReader entityReader, IDesktopFilePropertiesUpdater propertiesUpdater) : base(localizationProvider)
    {
        _propertiesUpdater = propertiesUpdater;
        MessageBus.Current.Listen<SetDesktopFileToEditMessage>()
            .Subscribe(async message => _desktopFile = message.Path is null
                ? null
                : await entityReader.ReadDesktopFileAsync(message.Path));
    }

    private void UpdateDesktopFileProperty(string propertyName, string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;
        
        ReflectionUpdateDesktopFileProperty(propertyName, value);
        _propertiesUpdater.RequireEntryUpdate(key, value!);
    }
    
    private void UpdateDesktopFileProperty(string propertyName, string key, bool? value)
    {
        ReflectionUpdateDesktopFileProperty(propertyName, value);
        _propertiesUpdater.RequireEntryUpdate(key, value.GetValueOrDefault());
    }

    private void ReflectionUpdateDesktopFileProperty(string propertyName, object? value)
    {
        Debug.Assert(_desktopFile is not null); 
        var desktopFileType = _desktopFile.GetType();
        var propertyInfo = desktopFileType.GetProperty(propertyName);
        Debug.Assert(propertyInfo is not null);
        propertyInfo.SetValue(_desktopFile, value);
        this.RaisePropertyChanged(propertyName);
    }
}