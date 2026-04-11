namespace GMenu.Modules.Configuration;

public class Configuration(ILogger logger) : IConfiguration
{
    private ObservableConfiguration? _configuration;
    private Channel<string?>? _channel;
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public ObservableConfiguration GetObservable()
    {
       Guard.EnsureMemberNotNull(_configuration, nameof(_configuration));
       return _configuration!;
    }

    public async Task EnsureExistsAsync()
    {
        if (File.Exists(StaticConfiguration.ConfigurationPath))
        {
            logger.Information("Configuration file already exists");
            await LoadConfigurationAsync();
            StartObserveConfiguration();
        }
        else
        {
            await CreateJsonFileAsync();
            StartObserveConfiguration();
        }
    }

    private async Task LoadConfigurationAsync()
    {
        await using var file = File.Open(StaticConfiguration.ConfigurationPath, FileMode.Open);
        _configuration = await JsonSerializer.DeserializeAsync<ObservableConfiguration>(file,
                ObservableConfigurationSerializerContext.Default.Options);
    }

    private async Task CreateJsonFileAsync()
    {
        try
        {
            await using var file = File.Create(StaticConfiguration.ConfigurationPath);
            await JsonSerializer.SerializeAsync(
                file, 
                StaticConfiguration.DefaultJsonConfiguration,
                DefaultConfigurationSerializerContext.Default.Options);
            logger.Information("Configuration file created in {path}", StaticConfiguration.ConfigurationPath);
            _configuration = new ObservableConfiguration()
            {
                User = StaticConfiguration.DefaultJsonConfiguration.User,
                SearchDesktopFilesDirectories = new ObservableCollection<DesktopFileDirectory>(StaticConfiguration.DefaultJsonConfiguration.DefaultDesktopFileDirectories.AsEnumerable()),
                UnexistingCategories = []
            };
        }
        catch(Exception exception)
        {
            if(File.Exists(StaticConfiguration.ConfigurationPath))
                File.Delete(StaticConfiguration.ConfigurationPath);
            logger.Error(exception, "Something go wrong during configuration creating");
            throw;
        }
    }

    private void StartObserveConfiguration()
    {
        _channel = Channel.CreateBounded<string?>(1);
       
        _configuration!.PropertyChanged += async void (_, args) =>
        {
            try
            {
                await _channel.Writer.WriteAsync(args.PropertyName);
            }
            catch (Exception e)
            {
                logger.Error(e, "Something go wrong during configuration update");
                throw;
            }
            logger.Information("Configuration key ({key}) will be changed in {path}", args.PropertyName, StaticConfiguration.ConfigurationPath);
            
        };
        _ = HandleJSONChangingAsync(_channel.Reader);
    }

    private async Task HandleJSONChangingAsync(ChannelReader<string?> reader)
    {
        await foreach (var propertyName in reader.ReadAllAsync())
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                await using var json = File.Open(StaticConfiguration.ConfigurationPath, FileMode.Open);
                await JsonSerializer.SerializeAsync(json, _configuration, ObservableConfigurationSerializerContext.Default.Options);
                logger.Debug("Configuration key ({key}) was fully changed in {path}", propertyName, StaticConfiguration.ConfigurationPath);
            }
            finally
            {
                _semaphoreSlim.Release();
            }

        }
    }
}