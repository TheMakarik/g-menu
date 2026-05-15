namespace GMenu.ViewModels;

public abstract  partial class ViewModelBase(ILocalizationProvider localizationProvider) : ReactiveValidationObject
{

    [Reactive] private ILocalizationProvider _localizationProvider  = localizationProvider;
    public Interaction<Unit, string> SelectTheDirectory { get; } = new();
    
}