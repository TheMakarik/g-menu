namespace GMenu.ViewModels;

public abstract  partial class ViewModelBase(ILocalizationProvider localizationProvider) : ReactiveObject
{

    [Reactive] private ILocalizationProvider _localizationProvider  = localizationProvider;
    public Interaction<ErrorType, Unit> ErrorInteraction { get; } = new();
    public Interaction<Unit, string> SelectTheDirectory { get; } = new();
    

   
}