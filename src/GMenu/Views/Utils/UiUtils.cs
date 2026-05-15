namespace GMenu.Views.Utils;

public static class UiUtils
{
    public static void BindInnerLeftContentForegroundToTextBoxMaterialUnderline(TextBox textBox)
    {
        var materialUnderLine = (MaterialUnderline)textBox.GetTemplateDescendants().First(visual => visual is MaterialUnderline);
        materialUnderLine.GetPropertyChangedObservable(MaterialUnderline.IsActiveProperty)
            .Subscribe(args =>
            {
                var innerLeftContent = textBox.InnerLeftContent as TemplatedControl;
                innerLeftContent?.Foreground = args.NewValue is true 
                    ? materialUnderLine.ActiveBrush
                    : materialUnderLine.IdleBrush;
            });
        materialUnderLine.GetPropertyChangedObservable(MaterialUnderline.IdleBrushProperty)
            .Subscribe(args =>
            {
                var innerLeftContent = textBox.InnerLeftContent as TemplatedControl;
                innerLeftContent?.Foreground = args.NewValue is true 
                    ? materialUnderLine.ActiveBrush
                    : materialUnderLine.IdleBrush;
            });
        
        materialUnderLine.GetPropertyChangedObservable(MaterialUnderline.ActiveBrushProperty)
            .Subscribe(args =>
            {
                var innerLeftContent = textBox.InnerLeftContent as TemplatedControl;
                innerLeftContent?.Foreground = args.NewValue is true 
                    ? materialUnderLine.ActiveBrush
                    : materialUnderLine.IdleBrush;
            });
    }
    
    public static void BindMaterialIconForegroundToTextBoxMaterialUnderline(TextBox textBox, MaterialIcon icon)
    {
        var materialUnderLine = (MaterialUnderline)textBox.GetTemplateDescendants().First(visual => visual is MaterialUnderline);
        materialUnderLine.GetPropertyChangedObservable(MaterialUnderline.IsActiveProperty)
            .Subscribe(args =>
            {
                icon.Foreground = args.NewValue is true 
                    ? materialUnderLine.ActiveBrush
                    : materialUnderLine.IdleBrush;
            });
        materialUnderLine.GetPropertyChangedObservable(MaterialUnderline.IdleBrushProperty)
            .Subscribe(args =>
            {
                icon.Foreground = materialUnderLine.IsActive
                    ? materialUnderLine.ActiveBrush
                    : materialUnderLine.IdleBrush;
            });
        materialUnderLine.GetPropertyChangedObservable(MaterialUnderline.ActiveBrushProperty)
            .Subscribe(args =>
            {
                icon.Foreground = materialUnderLine.IsActive
                    ? materialUnderLine.ActiveBrush
                    : materialUnderLine.IdleBrush;
            });
    }
}