namespace GMenu.ViewModels;

public partial class MainWindowViewModel(
    ILogger logger,
    IDesktopFileHeaderReader reader,
    IRootRequirer rootRequirer) : ViewModelBase
{
    public Interaction<ErrorType, Unit> ErrorInteraction { get; } = new(); 
  
    [ReactiveCommand]
    private async Task LoadCategoriesAsync()
    {
        var result = StartMethodWithRootRequirer(reader.GetAllHeaders, nameof(reader.GetAllHeaders));
    }


    private IObservable<T> StartMethodWithRootRequirer<T>(Func<T> method, string methodName)
    {
        return Observable.Start(method)
            .RetryWhen(exceptions => exceptions
                .SelectMany(async (Exception exception, int _) =>
                {
                    logger.Error(exception, "Exception occurred");

                    if (exception is not UnauthorizedAccessException)
                        throw exception;
                    
                    var source = new CancellationTokenSource();
                    await rootRequirer.RequireRootAsync(source);
                    return Unit.Default;
                }))
            .Catch<T, OperationCanceledException>(exception =>
            {
                logger.Error(exception, "User did not write superuser password, so cannot execute operation: {name}", methodName);
                ErrorInteraction.Handle(ErrorType.NoAccess);
                return Observable.Empty<T>();
            });
    }
    
}