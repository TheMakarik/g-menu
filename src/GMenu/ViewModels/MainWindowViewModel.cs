namespace GMenu.ViewModels;

public partial class MainWindowViewModel(IDesktopFileHeaderReader? reader = null) : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";

    [ReactiveCommand]
    private async Task LoadCategoriesAsync()
    {
        var source = new CancellationTokenSource();
        var headers = reader.GetAllHeadersAsync(source);
    }
}