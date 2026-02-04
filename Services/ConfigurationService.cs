using System.Text.Json;
using S7Dashboard.Models;

namespace S7Dashboard.Services;

public class ConfigurationService
{
    private readonly string _configPath;
    private readonly ILogger<ConfigurationService> _logger;
    private DashboardConfig _config = new();
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public event Action? ConfigurationChanged;

    public ConfigurationService(ILogger<ConfigurationService> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _configPath = Path.Combine(environment.ContentRootPath, "config.json");
        LoadConfiguration();
    }

    public DashboardConfig GetConfiguration() => _config;

    public void LoadConfiguration()
    {
        try
        {
            if (File.Exists(_configPath))
            {
                var json = File.ReadAllText(_configPath);
                _config = JsonSerializer.Deserialize<DashboardConfig>(json, _jsonOptions) ?? new DashboardConfig();
                _logger.LogInformation("Konfiguration geladen von {Path}", _configPath);
            }
            else
            {
                _config = CreateDefaultConfiguration();
                SaveConfiguration();
                _logger.LogInformation("Standard-Konfiguration erstellt");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Laden der Konfiguration");
            _config = new DashboardConfig();
        }
    }

    public void SaveConfiguration()
    {
        try
        {
            var json = JsonSerializer.Serialize(_config, _jsonOptions);
            File.WriteAllText(_configPath, json);
            _logger.LogInformation("Konfiguration gespeichert nach {Path}", _configPath);
            ConfigurationChanged?.Invoke();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Speichern der Konfiguration");
            throw;
        }
    }

    public void UpdatePlcConfiguration(PlcConfiguration plcConfig)
    {
        _config.PlcConfiguration = plcConfig;
        SaveConfiguration();
    }

    public void AddDataPoint(DataPointConfig dataPoint)
    {
        _config.DataPoints.Add(dataPoint);
        SaveConfiguration();
    }

    public void UpdateDataPoint(DataPointConfig dataPoint)
    {
        var index = _config.DataPoints.FindIndex(d => d.Id == dataPoint.Id);
        if (index >= 0)
        {
            _config.DataPoints[index] = dataPoint;
            SaveConfiguration();
        }
    }

    public void DeleteDataPoint(string id)
    {
        _config.DataPoints.RemoveAll(d => d.Id == id);
        _config.Tiles.RemoveAll(t => t.DataPointId == id);
        SaveConfiguration();
    }

    public void AddTile(TileConfig tile)
    {
        _config.Tiles.Add(tile);
        SaveConfiguration();
    }

    public void UpdateTile(TileConfig tile)
    {
        var index = _config.Tiles.FindIndex(t => t.Id == tile.Id);
        if (index >= 0)
        {
            _config.Tiles[index] = tile;
            SaveConfiguration();
        }
    }

    public void DeleteTile(string id)
    {
        _config.Tiles.RemoveAll(t => t.Id == id);
        SaveConfiguration();
    }

    public void UpdateGridSize(int columns, int rows)
    {
        _config.GridColumns = columns;
        _config.GridRows = rows;
        SaveConfiguration();
    }

    private DashboardConfig CreateDefaultConfiguration()
    {
        return new DashboardConfig
        {
            PlcConfiguration = new PlcConfiguration
            {
                IpAddress = "192.168.0.1",
                Rack = 0,
                Slot = 1,
                PollingIntervalMs = 1000
            },
            DataPoints = new List<DataPointConfig>
            {
                new()
                {
                    Id = "dp1",
                    Name = "Temperatur",
                    DbNumber = 1,
                    StartByte = 0,
                    DataType = "Real",
                    Unit = "°C",
                    ScaleFactor = 1.0,
                    Offset = 0.0,
                    DecimalPlaces = 1
                },
                new()
                {
                    Id = "dp2",
                    Name = "Druck",
                    DbNumber = 1,
                    StartByte = 4,
                    DataType = "Real",
                    Unit = "bar",
                    ScaleFactor = 1.0,
                    Offset = 0.0,
                    DecimalPlaces = 2
                }
            },
            Tiles = new List<TileConfig>
            {
                new()
                {
                    Id = "tile1",
                    DataPointId = "dp1",
                    PositionX = 1,
                    PositionY = 1,
                    Width = 1,
                    Height = 1,
                    TitleFontSize = 16,
                    ValueFontSize = 32,
                    BackgroundColor = "#2196F3"
                },
                new()
                {
                    Id = "tile2",
                    DataPointId = "dp2",
                    PositionX = 2,
                    PositionY = 1,
                    Width = 1,
                    Height = 1,
                    TitleFontSize = 16,
                    ValueFontSize = 32,
                    BackgroundColor = "#4CAF50"
                }
            },
            GridColumns = 4,
            GridRows = 3
        };
    }
}
