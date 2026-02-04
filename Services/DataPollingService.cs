using S7Dashboard.Models;

namespace S7Dashboard.Services;

public class DataPollingService : BackgroundService
{
    private readonly S7CommunicationService _s7Service;
    private readonly ConfigurationService _configService;
    private readonly ILogger<DataPollingService> _logger;
    private int _pollingInterval = 1000;

    public event Action<Dictionary<string, double?>>? ValuesUpdated;
    public event Action<bool>? ConnectionStateChanged;

    public Dictionary<string, double?> CurrentValues { get; private set; } = new();
    public bool IsConnected => _s7Service.IsConnected;
    public string? LastError => _s7Service.LastError;

    public DataPollingService(
        S7CommunicationService s7Service,
        ConfigurationService configService,
        ILogger<DataPollingService> logger)
    {
        _s7Service = s7Service;
        _configService = configService;
        _logger = logger;

        _configService.ConfigurationChanged += OnConfigurationChanged;
    }

    private void OnConfigurationChanged()
    {
        var config = _configService.GetConfiguration();
        _pollingInterval = config.PlcConfiguration.PollingIntervalMs;
    }

    public async Task ConnectAsync()
    {
        var config = _configService.GetConfiguration();
        _pollingInterval = config.PlcConfiguration.PollingIntervalMs;

        var connected = await _s7Service.ConnectAsync(config.PlcConfiguration);
        ConnectionStateChanged?.Invoke(connected);
    }

    public void Disconnect()
    {
        _s7Service.Disconnect();
        ConnectionStateChanged?.Invoke(false);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DataPollingService gestartet");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_s7Service.IsConnected)
                {
                    var config = _configService.GetConfiguration();
                    var values = await _s7Service.ReadAllValuesAsync(config.DataPoints);

                    CurrentValues = values;
                    ValuesUpdated?.Invoke(values);
                }
                else
                {
                    // Versuche automatisch neu zu verbinden
                    await TryReconnectAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler im Polling-Zyklus");
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }
    }

    private async Task TryReconnectAsync()
    {
        var config = _configService.GetConfiguration();
        if (!string.IsNullOrEmpty(config.PlcConfiguration.IpAddress))
        {
            _logger.LogInformation("Versuche Wiederverbindung...");
            var connected = await _s7Service.ConnectAsync(config.PlcConfiguration);
            ConnectionStateChanged?.Invoke(connected);
        }
    }

    public override void Dispose()
    {
        _configService.ConfigurationChanged -= OnConfigurationChanged;
        _s7Service.Dispose();
        base.Dispose();
    }
}
