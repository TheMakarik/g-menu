using ILogger = Serilog.ILogger;

namespace GMenu.ViewModels;

public abstract class ViewModelBase(ILogger logger, IRootRequirer rootRequirer, ILocalizationProvider localizationProvider) : ReactiveObject
{

    [Reactive] public ILocalizationProvider LocalizationProvider { get; } = localizationProvider;
    public Interaction<ErrorType, Unit> ErrorInteraction { get; } = new();
    public Interaction<Unit, string> SelectTheDirectory { get; } = new();
    public Interaction<IObservable<object>, Task<Task<object>>> AfterApplicationFullyLoading { get; } = new();
    
    protected IObservable<T> WithRootRequire<T>(IObservable<T> method, string methodName) where T : class
    {
        return method.RetryWhen(exceptions => exceptions
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

    protected async Task<T> AfterApplicationLoadedAsync<T>(IObservable<T> observable) where T : class
    {
        return (T) await await await AfterApplicationFullyLoading.Handle(observable);
    }
}