namespace GMenu.Views;

public partial class DesktopFilesTreeView : ReactiveUserControl<DesktopFilesTreeViewModel>
{
    public const string DesktopFileContextMenuResourceKey = "ContextMenuForFile";
    [DynamicDependency( DynamicallyAccessedMemberTypes.PublicEvents, typeof(TreeView))]
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
       
       Console.WriteLine("SDGKLKDSLL");
       treeViewItem.Tag = dispatcherTimer;
    }

    private void StopDrag(object sender, PointerReleasedEventArgs e)
    {
        var treeViewItem = ((Control)sender).GetLogicalParent<TreeViewItem>() ?? throw new InvalidOperationException("Cannot found tree view item");
        (treeViewItem.Tag as DispatcherTimer)?.Stop();
        treeViewItem?.Tag = null;
    }

    private void LoadContextMenuAndDragAndDrop(object sender, RoutedEventArgs e)
    { 
        var control = (Control)sender;
        var treeView = control.GetLogicalParent<TreeViewItem>() ?? throw new InvalidOperationException("Cannot found parent");
        LoadContextMenu(treeView, control);
        LoadDragAndDrop(treeView);

    }

    private void LoadDragAndDrop(TreeViewItem treeViewItem)
    {
      treeViewItem.PointerPressed += (sender, args) => {StartDrag(sender!, args);};
    }

    private void LoadContextMenu(TreeViewItem treeViewItem, Control control)
    {
        treeViewItem.ContextMenu = control.Resources[DesktopFileContextMenuResourceKey] as ContextMenu;
    }
}