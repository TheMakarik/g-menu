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
            logger.Information("Configuration key ({key}) will be  changed in {path}", args.PropertyName, StaticConfiguration.ConfigurationPath);
            
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
                if (propertyName is null)
                    return;

                var json = File.Open(StaticConfiguration.ConfigurationPath, FileMode.Open);
                var jsonNode = await JsonNode.ParseAsync(json);
        
                if (IsArrayValueUpdated(propertyName, out var index, out var name))
                {
                    var newValue = ((IList)_configuration!.GetType()
                        .GetProperty(name)?.GetValue(_configuration)!)[index];
            
                    var arrayNode = jsonNode![name]?.AsArray();
                    if (arrayNode != null)
                    {
                        //May have some problems with NativeAOT so check that all possible types of new object is primitives or has System.Diagnostic.CodeAnalyses.DynamicallyAccessedMembersAttribute
                        var newNode = JsonSerializer.SerializeToNode(newValue);
                        arrayNode[index] = newNode;
                    }
                }
                else
                {
                    var newValue = _configuration!.GetType()
                        .GetProperty(propertyName)?.GetValue(_configuration)!;
            
                    //May have some problems with NativeAOT so check that all possible types of new object is primitives or has System.Diagnostic.CodeAnalyses.DynamicallyAccessedMembersAttribute
                    var newNode = JsonSerializer.SerializeToNode(newValue);
                    jsonNode![propertyName] = newNode;
                }
            
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
            
                var updatedJson = jsonNode!.ToJsonString(options);
                await File.WriteAllTextAsync(StaticConfiguration.ConfigurationPath, updatedJson);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
           
        }
    }

    private bool IsArrayValueUpdated(string? propertyName, out int index, [NotNullWhen(true)] out string? name)
    {
        index = 0;
        name = null;
        if (propertyName is null)
            return false;
        
        if (!propertyName.EndsWith("]"))
            return false;
        
        //Minus one because indexes are zero based and minus one again because we have just checked last charecter
        var startI = propertyName.Length - 2;
        for (var i = startI; i >= 0; i--)
        {
            if (propertyName[i] == '[')
            {
                if (i == startI)
                    return false;
                
                index = i - 1;
                name = propertyName[..i];
                return true;
                
            }
            if (!int.TryParse(propertyName[i].ToString(), out var _))
                return false;
        }
        return false;

    }
}