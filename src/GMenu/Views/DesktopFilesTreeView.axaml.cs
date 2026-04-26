namespace GMenu.Views;

public partial class DesktopFilesTreeView : ReactiveUserControl<DesktopFilesTreeViewModel>
{
    public DesktopFilesTreeView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<DesktopFilesTreeViewModel>();
        
    }

    private void StartDrag(object sender, PointerPressedEventArgs e)
    {
       var treeViewItem = ((Control)sender).GetLogicalParent<TreeViewItem>() ?? throw new InvalidOperationException("Cannot found tree view item");

       var dispatcherTimer = new DispatcherTimer() {  };

       dispatcherTimer.Tick += (_, _) =>
       {

       };
       
       treeViewItem.Tag = dispatcherTimer;
    }

    private void StopDrag(object sender, PointerReleasedEventArgs e)
    {
        var treeViewItem = ((Control)sender).GetLogicalParent<TreeViewItem>() ?? throw new InvalidOperationException("Cannot found tree view item");
        (treeViewItem.Tag as DispatcherTimer)?.Stop();
        treeViewItem?.Tag = null;
    }
}