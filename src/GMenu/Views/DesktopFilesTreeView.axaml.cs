namespace GMenu.Views;
public partial class DesktopFilesTreeView : ReactiveUserControl<DesktopFilesTreeViewModel>
{
    public const string DesktopFileContextMenuResourceKey = "ContextMenuForFile";
    private TreeViewItem? _lastSelectedItem;
    
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicEvents, typeof(TreeView))]
    public DesktopFilesTreeView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<DesktopFilesTreeViewModel>();

        ViewModel!.GoDownInTheDesktopFilesView.RegisterHandler(context =>
        {
            GoDownInTheDesktopFilesView();
            context.SetOutput(Unit.Default);
        });
        ViewModel!.GoUpInTheDesktopFilesView.RegisterHandler(context =>
        {
            GoUpInTheDesktopFilesView();
            context.SetOutput(Unit.Default);
        });
    }

    private void GoUpInTheDesktopFilesView()
    {
        if (DesktopFilesListBoxControl.IsVisible)
            NavigateInListBoxUp();
        else
            GoUpInTreeView();
    }
    
    private void GoDownInTheDesktopFilesView()
    {
        if (DesktopFilesListBoxControl.IsVisible)
            NavigateInListBoxDown();
        else
            GoDownInTreeView();
    }
    
    private void NavigateInListBoxUp()
    {
        if (DesktopFilesListBoxControl.SelectedIndex <= 0)
            DesktopFilesListBoxControl.SelectedIndex = DesktopFilesListBoxControl.Items.Count - 1;
        else
            DesktopFilesListBoxControl.SelectedIndex--;
        
        if (DesktopFilesListBoxControl.SelectedItem is not null)
            DesktopFilesListBoxControl.ScrollIntoView(DesktopFilesListBoxControl.SelectedItem);
    }
    
    private void NavigateInListBoxDown()
    {
        if (DesktopFilesListBoxControl.SelectedIndex >= DesktopFilesListBoxControl.Items.Count - 1)
        {
            DesktopFilesListBoxControl.SelectedIndex = 0;
        }
        else
        {
            DesktopFilesListBoxControl.SelectedIndex++;
        }

        if (DesktopFilesListBoxControl.SelectedItem is not  null)
            DesktopFilesListBoxControl.ScrollIntoView(DesktopFilesListBoxControl.SelectedItem);
    }
    
    private void GoUpInTreeView()
    {
        if (!DesktopFilesTreeViewControl.IsVisible)
            return;

        var treeViewItems = GetRootItems();
            
        if (!HasAnySelected(treeViewItems))
        {
            SetLastSelected(treeViewItems);
            return;
        }
        
        var allItems = GetAllItemsFlat(treeViewItems);
        var currentIndex = FindCurrentSelectedIndex(allItems);
        
        switch (currentIndex)
        {
            case > 0:
            {
                ClearAllSelections(treeViewItems);
                var prevItem = allItems[currentIndex - 1];
                ExpandToItem(prevItem, treeViewItems);
                prevItem.IsSelected = true;
                break;
            }
            case 0:
            {
                ClearAllSelections(treeViewItems);
                var lastItem = allItems[^1];
                ExpandToItem(lastItem, treeViewItems);
                lastItem.IsSelected = true;
                break;
            }
        }
    }
    
    private void GoDownInTreeView()
    {
        if (!DesktopFilesTreeViewControl.IsVisible)
            return;

        var treeViewItems = GetRootItems();
            
        if (!HasAnySelected(treeViewItems))
        {
            SetFirstSelected(treeViewItems);
            return;
        }
        
        var allItems = GetAllItemsFlat(treeViewItems);
        var currentIndex = FindCurrentSelectedIndex(allItems);
        
        if (currentIndex >= 0 && currentIndex < allItems.Count - 1)
        {
            ClearAllSelections(treeViewItems);
            var nextItem = allItems[currentIndex + 1];
            ExpandToItem(nextItem, treeViewItems);
            nextItem.IsSelected = true;
        }
        else if (currentIndex == allItems.Count - 1)
        {
            ClearAllSelections(treeViewItems);
            var firstItem = allItems[0];
            ExpandToItem(firstItem, treeViewItems);
            firstItem.IsSelected = true;
        }
    }

    private IReadOnlyList<TreeViewItem> GetRootItems() =>
        DesktopFilesTreeViewControl
            .Items
            .Select(item => DesktopFilesTreeViewControl.ContainerFromItem(item))
            .Cast<TreeViewItem>()
            .ToArray();

    private bool HasAnySelected(IReadOnlyList<TreeViewItem> items)
    {
        foreach (var item in items)
        {
            if (item.IsSelected) return true;
            if (GetSelectableChildren(item).Any(static child => child.IsSelected)) return true;
        }
        return false;
    }

    private void SetFirstSelected(IReadOnlyList<TreeViewItem> treeViewItems)
    {
        if (treeViewItems.Count == 0) return;
        
        ClearAllSelections(treeViewItems);
        var firstItem = treeViewItems[0];
        ExpandToDeepestFirst(firstItem, treeViewItems);
        firstItem.IsSelected = true;
        _lastSelectedItem = firstItem;
    }
    
    private void SetLastSelected(IReadOnlyList<TreeViewItem> treeViewItems)
    {
        if (treeViewItems.Count == 0) return;
        
        ClearAllSelections(treeViewItems);
        var lastItem = treeViewItems[^1];
        ExpandToDeepestLast(lastItem, treeViewItems);
        lastItem.IsSelected = true;
        _lastSelectedItem = lastItem;
    }

    private void ExpandToDeepestFirst(TreeViewItem item, IReadOnlyList<TreeViewItem> rootItems)
    {
        while (true)
        {
            var children = GetSelectableChildren(item);

            if (children.Count > 0)
            {
                item.IsExpanded = true;
                item = children[0];
                continue;
            }
            item.IsSelected = true;
            break;
        }
    }

    private void ExpandToDeepestLast(TreeViewItem item, IReadOnlyList<TreeViewItem> rootItems)
    {
        while (true)
        {
            var children = GetSelectableChildren(item);

            if (children.Count > 0)
            {
                item.IsExpanded = true;
                item = children[^1];
                continue;
            }
            else
                item.IsSelected = true;

            break;
        }
    }

    private void ExpandToItem(TreeViewItem target, IReadOnlyList<TreeViewItem> rootItems)
    {
        var path = GetPathToItem(target, rootItems);
        foreach (var item in path)
            item.IsExpanded = true;
        
        
        if (_lastSelectedItem != null && _lastSelectedItem != target)
            CollapseUnusedPath(_lastSelectedItem, target, rootItems);
        
        
        _lastSelectedItem = target;
    }
    
    private IReadOnlyList<TreeViewItem> GetPathToItem(TreeViewItem target, IReadOnlyList<TreeViewItem> rootItems)
    {
        var path = new List<TreeViewItem>();
        
        foreach (var root in rootItems)
            if (FindPathInChildren(root, target, path))
                return path;
        
        return path;
    }
    
    private bool FindPathInChildren(TreeViewItem current, TreeViewItem target, List<TreeViewItem> path)
    {
        if (current == target)
            return true;
        
        var children = GetSelectableChildren(current);
        if (!children.Any(child => FindPathInChildren(child, target, path)))
            return false;
        
        path.Insert(0, current);
        return true;

    }
    
    private void CollapseUnusedPath(TreeViewItem oldItem, TreeViewItem newItem, IReadOnlyList<TreeViewItem> rootItems)
    {
        var oldPath = GetPathToItem(oldItem, rootItems);
        var newPath = GetPathToItem(newItem, rootItems);
        
        var commonPrefixLength = 0;
        while (commonPrefixLength < oldPath.Count && 
               commonPrefixLength < newPath.Count && 
               oldPath[commonPrefixLength] == newPath[commonPrefixLength])
        {
            commonPrefixLength++;
        }
        
        for (var i = oldPath.Count - 1; i >= commonPrefixLength; i--)
        {
            var parent = oldPath[i];
            var hasSelectedDescendant = GetSelectableChildren(parent).Any(child => IsDescendantSelected(child, newItem));

            if (!hasSelectedDescendant)
                parent.IsExpanded = false;
        }
    }
    
    private bool IsDescendantSelected(TreeViewItem candidate, TreeViewItem selected)
    {
        return candidate == selected || GetSelectableChildren(candidate).Any(child => IsDescendantSelected(child, selected));
    }
    
    private IReadOnlyList<TreeViewItem> GetAllItemsFlat(IReadOnlyList<TreeViewItem> rootItems)
    {
        var result = new List<TreeViewItem>();
        
        foreach (var item in rootItems)
        {
            result.Add(item);
            result.AddRange(GetAllChildrenFlat(item));
        }
        
        return result;
    }
    
    private IReadOnlyList<TreeViewItem> GetAllChildrenFlat(TreeViewItem parent)
    {
        var result = new List<TreeViewItem>();
        var children = GetSelectableChildren(parent);
        
        foreach (var child in children)
        {
            result.Add(child);
            result.AddRange(GetAllChildrenFlat(child));
        }
        
        return result;
    }
    
    private static int FindCurrentSelectedIndex(IReadOnlyList<TreeViewItem> allItems)
    {
        for (var i = 0; i < allItems.Count; i++)
            if (allItems[i].IsSelected)
                return i;
        return -1;
    }
    
    private void ClearAllSelections(IReadOnlyList<TreeViewItem> items)
    {
        foreach (var item in items)
        {
            item.IsSelected = false;
            var children = GetAllChildrenFlat(item);
            foreach (var child in children)
                child.IsSelected = false;
        }
    }
    
    private IReadOnlyList<TreeViewItem> GetSelectableChildren(TreeViewItem parent) =>
        parent.GetLogicalChildren().OfType<TreeViewItem>().ToArray();

    private void LoadContextMenu(object sender, RoutedEventArgs e)
    { 
        var control = (Control)sender;
        var item = control.GetLogicalParent() ?? throw new InvalidOperationException("Cannot found selectable item");
        ((Control)item).ContextMenu = control.Resources[DesktopFileContextMenuResourceKey] as ContextMenu;
    }
}