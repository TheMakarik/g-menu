namespace GMenu.ViewModels;

public partial class OpenableFilesViewModel(string path, bool isOpen, ILogger logger, IRootRequirer rootRequirer, ILocalizationProvider localizationProvider) : ViewModelBase(logger, rootRequirer, localizationProvider)
{
    [Reactive] private string _path = path;
    [Reactive] private bool _isOpen = isOpen;
}