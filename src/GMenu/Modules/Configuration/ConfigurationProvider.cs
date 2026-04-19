using ILogger = Serilog.ILogger;

namespace GMenu.Modules.Configuration;

public class ConfigurationProvider(ILogger logger) : IConfigurationProvider
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
#pragma warning restore IL2026
#pragma warning restore IL3050
        logger.Information("Configuration loaded successfully: {configurationProvider}", JsonSerializer.Serialize(_configuration, ObservableConfigurationSerializerContext.Default.Options));
    }

    private async Task CreateJsonFileAsync()
    {
        try
        {
            if(!Directory.Exists(StaticConfiguration.ConfigurationDirectory))
                Directory.CreateDirectory(StaticConfiguration.ConfigurationDirectory);

            await using var file = File.Create(StaticConfiguration.ConfigurationPath);
#pragma warning disable IL2026
#pragma warning disable IL3050
            await JsonSerializer.SerializeAsync(
                file, 
                StaticConfiguration.DefaultJsonConfiguration,
                ObservableConfigurationSerializerContext.Default.Options);
#pragma warning restore IL2026
#pragma warning restore IL3050
            logger.Information("Configuration file created in {path}", StaticConfiguration.ConfigurationPath);
            _configuration = StaticConfiguration.DefaultJsonConfiguration;
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
        var backgroundWorker = new BackgroundWorker();
        backgroundWorker.DoWork += (_, _) =>
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
            _ = HandleJSONChangingAsync(_channel.Reader);
        };
        
        backgroundWorker.RunWorkerAsync();
    }

    private async Task HandleJSONChangingAsync(ChannelReader<string?> reader)
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