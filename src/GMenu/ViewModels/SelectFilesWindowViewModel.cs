namespace GMenu.ViewModels;

public partial class SelectFilesWindowViewModel(
    ILocalizationProvider provider,
    ILogger logger)
    : ViewModelBase(provider)
{

    [Reactive]
    private ObservableCollection<OpenableFilesViewModel> _pathsToSelect = [];

    [Reactive] private OpenableFilesViewModel? _selectedItem;

    private readonly ILogger _logger = logger;

    public Interaction<string, Unit> OpenDirectory { get; } = new();

    [ReactiveCommand]
    private void LoadFiles(string[] paths)
    {
        PathsToSelect = new ObservableCollection<OpenableFilesViewModel>(paths
            .Select(path => new OpenableFilesViewModel(
                path,
                isOpen: false, 
                LocalizationProvider)));
        _logger.Debug("Loaded directories to select: {directories}", paths);
        this.WhenPropertyChanged(prop => prop.SelectedItem)
            .Subscribe(async  _ => await SelectDirectoryAsync(SelectedItem));
    }
    
    [ReactiveCommand]
    private void CloseDirectory(string path)
    {
        PathsToSelect.First(pathToSelect => pathToSelect.Path == path).IsOpen = false;
        logger.Debug("Closing directory: {directory}", path);
    }
    
    private async Task SelectDirectoryAsync(OpenableFilesViewModel? parameter)
    {
        if(parameter is null)
            return;
        
        var path = parameter.Path;
        _logger.Debug("Selecting directory: {directory}", path);
        foreach (var openableFilesViewModel in PathsToSelect)
            openableFilesViewModel.IsOpen = openableFilesViewModel.Path == path;
        await OpenDirectory.Handle(path);
    }
}