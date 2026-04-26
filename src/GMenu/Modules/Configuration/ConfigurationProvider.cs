namespace GMenu.Modules.Configuration;

public class ConfigurationProvider(ILogger logger, GMenuOptions options) : IConfigurationProvider
{
    private ObservableConfiguration? _configuration;
    private Channel<string?>? _channel;
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    
    public  ObservableConfiguration CurrentObservable => _configuration ?? throw new InvalidOperationException("Configuration not initialized");

    public async Task EnsureExistsAsync()
    {
        if (File.Exists(StaticConfiguration.ConfigurationPath))
        {
            logger.Information("Configuration file already exists");
            await LoadConfigurationAsync();
        }
        else
        {
            await CreateJsonFileAsync();
        }
        
        StartObserveConfiguration();
    }

    private async Task LoadConfigurationAsync()
    {
        await using var file = File.Open(StaticConfiguration.ConfigurationPath, FileMode.Open);
#pragma warning disable IL2026
#pragma warning disable IL3050
        _configuration = await JsonSerializer.DeserializeAsync<ObservableConfiguration>(file, ObservableConfigurationSerializerContext.Default.Options);
        logger.Information("Configuration loaded successfully: {configurationProvider}", JsonSerializer.Serialize(_configuration, ObservableConfigurationSerializerContext.Default.Options));
#pragma warning restore IL2026
#pragma warning restore IL3050
    }

    private async Task CreateJsonFileAsync()
    {
        try
        {
            if(!Directory.Exists(options.Configuration.Directory))
                Directory.CreateDirectory(options.Configuration.Directory);

            await using var file = File.Create(StaticConfiguration.ConfigurationPath);
            var defaultConfiguration = new ObservableConfiguration()
            {
                Language = CultureInfo.CurrentCulture,
                UnexistingCategories = [],
                AccentColor = null,
            };
#pragma warning disable IL2026
#pragma warning disable IL3050
            await JsonSerializer.SerializeAsync(
                file, 
                defaultConfiguration,
                ObservableConfigurationSerializerContext.Default.Options);
#pragma warning restore IL2026
#pragma warning restore IL3050
            logger.Information("Configuration file created in {path}", StaticConfiguration.ConfigurationPath);
            _configuration = defaultConfiguration;
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
        Task.Run(() => 
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

                logger.Information("Configuration key ({key}) will be changed in {path}", args.PropertyName,
                    StaticConfiguration.ConfigurationPath);

            };
            _ = HandleJsonChangingAsync(_channel.Reader);
        });
    }

    private async Task HandleJsonChangingAsync(ChannelReader<string?> reader)
    {
        await foreach (var propertyName in reader.ReadAllAsync())
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                await using var json = File.Open(StaticConfiguration.ConfigurationPath, FileMode.Open);
#pragma warning disable IL2026
#pragma warning disable IL3050
                await JsonSerializer.SerializeAsync(json, _configuration, ObservableConfigurationSerializerContext.Default.Options);
#pragma warning restore IL2026
#pragma warning restore IL3050
                logger.Debug("Configuration key ({key}) was fully changed in {path}", propertyName, StaticConfiguration.ConfigurationPath);
            }
            finally
            {
                _semaphoreSlim.Release();
            }

        }
    }
    
}