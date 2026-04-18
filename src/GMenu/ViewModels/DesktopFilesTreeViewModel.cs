using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public sealed partial class DesktopFilesTreeViewModel(
    IDesktopFileHeaderReader reader, 
    ILogger logger, 
    IConfigurationProvider configuration,
    IDesktopFileIconPathRefiner iconPathRefiner,
    ILocalizationProvider localizationProvider,
    IRootRequirer rootRequirer) : ViewModelBase(logger, rootRequirer, localizationProvider)
{
    [Reactive] private ObservableCollection<TreeViewModelBase> _children = [];
    
    private readonly ILogger _logger = logger;
    private readonly IRootRequirer _rootRequirer = rootRequirer;

    [ReactiveCommand]
    private async Task LoadDesktopFilesAsync()
    {
        var result = await WithRootRequire(
            Observable.Start(reader.GetAllHeaders), 
            nameof(reader.GetAllHeaders));
        
        iconPathRefiner.StartBackgroundIconsLoading(result.Where(header => header.IconPath is not null).Select(header => header.IconPath!));
        MessageBus.Current.SendMessage(new SetDesktopFilesCountMessage(){FilesCount = result.Count});
        
        var configuredPaths = configuration.CurrentObservable.SearchDesktopFilesDirectories
            .ToHashSet();
        
        var allGroups = result
            .GroupBy(header => header.Directory)
            .ToDictionary(group => group.Key, g => g.ToList());
        
        var allPaths = allGroups.Keys
            .Where(path => configuredPaths.Any(configuredPath => path.StartsWith(configuredPath.Path)))
            .OrderBy(path => path.Length)
            .ToList();

        var viewModels = new Dictionary<string, TreeViewModelDirectories>();

        foreach (var path in allPaths)
        {
            var grouping = result
                .GroupBy(header => header.Directory)
                .First(group => group.Key == path);
            
            var viewModel = new TreeViewModelDirectories(
                new DirectoryTreeViewInfo { Path = path, Headers =  grouping, LocalizationKey = configuredPaths.FirstOrDefault(searchPath => searchPath.Path == path).LocalizationKey },
                iconPathRefiner, 
                _logger,
                localizationProvider,
                _rootRequirer);
            
            viewModels[path] = viewModel;

            if (!configuredPaths.Select(paths => paths.Path).Contains(path))
            {
                var lastSlash = path.LastIndexOf(Path.DirectorySeparatorChar);
                var parentPath = lastSlash > 0 ? path[..lastSlash] : null;

                if (parentPath is not null && viewModels.TryGetValue(parentPath, out var parent))
                    parent.Children.Insert(0, viewModel);
            }
            else
                _children.Insert(0, viewModel);
        }
        
    }
}