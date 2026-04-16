using Avalonia.Logging;
using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public sealed partial class DesktopFilesTreeViewModel(
    IDesktopFileHeaderReader reader, 
    ILogger logger, 
    IConfigurationProvider configuration,
    IDesktopFileIconPathRefiner iconPathRefiner,
    IRootRequirer rootRequirer) : ViewModelBase(logger, rootRequirer)
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
        
        var configuredPaths = configuration.CurrentObservable.SearchDesktopFilesDirectories
            .Select(directory => directory.Path)
            .ToHashSet();
        
        var allGroups = result
            .GroupBy(header => header.Directory)
            .ToDictionary(group => group.Key, g => g.ToList());
        
        var allPaths = allGroups.Keys
            .Where(path => configuredPaths.Any(path.StartsWith))
            .OrderBy(path => path.Length)
            .ToList();

        var viewModels = new Dictionary<string, TreeViewModelDirectories>();

        foreach (var path in allPaths)
        {
            var grouping = result
                .GroupBy(header => header.Directory)
                .First(group => group.Key == path);
                
            var viewModel = new TreeViewModelDirectories(
                new DirectoryTreeViewInfo { Path = path, Headers =  grouping },
                iconPathRefiner, 
                _logger,
                _rootRequirer);
            
            viewModels[path] = viewModel;

            if (!configuredPaths.Contains(path))
            {
                var lastSlash = path.LastIndexOf(Path.DirectorySeparatorChar);
                var parentPath = lastSlash > 0 ? path[..lastSlash] : null;

                if (parentPath is not null && viewModels.TryGetValue(parentPath, out var parent))
                    parent.Children.Add(viewModel);
            }
            else
                _children.Add(viewModel);
        }
        
    }
}