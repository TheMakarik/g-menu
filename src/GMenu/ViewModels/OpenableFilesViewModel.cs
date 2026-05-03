namespace GMenu.ViewModels;

public partial class OpenableFilesViewModel(string path, bool isOpen, ILocalizationProvider localizationProvider) : ViewModelBase(localizationProvider)
{
    [Reactive] private string _path = path;
    [Reactive] private bool _isOpen = isOpen;
}