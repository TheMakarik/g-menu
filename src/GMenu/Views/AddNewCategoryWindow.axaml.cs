
namespace GMenu.Views;

public partial class AddNewCategoryWindow :  ReactiveWindow<AddNewCategoryWindowViewModel>
{
    private static ReadOnlyCollection<string>? _materialIconCache;

    [DynamicDependency(DynamicallyAccessedMemberTypes.AllEvents, typeof(Window))]
    public AddNewCategoryWindow()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<AddNewCategoryWindowViewModel>();

        ViewModel!.GetIconsInteraction.RegisterHandler(context =>
        {
            var icons = _materialIconCache ?? Enum.GetNames<MaterialIconKind>().AsReadOnly();
            _materialIconCache ??= icons;
            
            var output = string.IsNullOrWhiteSpace(context.Input)
                ? icons
                : icons.Where(kindString => kindString.Contains(context.Input, StringComparison.OrdinalIgnoreCase));
            
            context.SetOutput(output);
        });
    }

    private void LoadForegroundBindingsForLocalizedCategory(object? sender, RoutedEventArgs e)
    {
        UiUtils.BindMaterialIconForegroundToTextBoxMaterialUnderline(LocalizedCategoryTextBox, LocalizedCategoryIcon);
    }
    
    private void LoadForegroundBindingsForUnlocalizedCategory(object? sender, RoutedEventArgs e)
    {
        UiUtils.BindMaterialIconForegroundToTextBoxMaterialUnderline(UnlocalizedCategoryTextBox, UnlocalizedCategoryIcon);
    }

    private void ToggleMaterialIconsPopup(object? sender, RoutedEventArgs e)
    {
        var child = (Border)MaterialIconsPopup.Child!;
        child.Background = this.Background;
        child.BorderBrush = this.BorderBrush;
        MaterialIconsPopup.IsOpen = !MaterialIconsPopup.IsOpen;
    }
    

    private async void SetIconKind(object? sender, RoutedEventArgs e)
    {
        var button = (Button)sender!;
        await ViewModel!.SetIconKindCommand.Execute((string)button.CommandParameter!);
        
    }

    private void LoadInnerIconForeground(object sender, RoutedEventArgs e)
    {
       UiUtils.BindInnerLeftContentForegroundToTextBoxMaterialUnderline((TextBox)sender);
    }
}