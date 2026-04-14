namespace GMenu.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        
        this.WhenActivated(action =>
        {
          ViewModel.ErrorInteraction.RegisterHandler(context => )
        });
    }
}