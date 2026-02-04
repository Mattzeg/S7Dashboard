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

                // Migration: Legacy-Konfiguration zu neuem Format
                MigrateLegacyConfig();

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

    private void MigrateLegacyConfig()
    {
        // Wenn es alte Tiles gibt aber keine Dashboards, migrieren
        if (_config.Tiles != null && _config.Tiles.Any() && !_config.Dashboards.Any())
        {
            _config.Dashboards.Add(new Dashboard
            {
                Id = "default",
                Name = "Haupt-Dashboard",
                GridColumns = _config.GridColumns ?? 4,
                GridRows = _config.GridRows ?? 3,
                Tiles = _config.Tiles
            });

            // Legacy-Felder leeren
            _config.Tiles = null;
            _config.GridColumns = null;
            _config.GridRows = null;

            SaveConfiguration();
            _logger.LogInformation("Legacy-Konfiguration migriert");
        }

        // Sicherstellen, dass mindestens ein Dashboard existiert
        if (!_config.Dashboards.Any())
        {
            _config.Dashboards.Add(new Dashboard
            {
                Id = "default",
                Name = "Haupt-Dashboard",
                GridColumns = 4,
                GridRows = 3,
                Tiles = new List<TileConfig>()
            });
        }
    }

    public void SaveConfiguration()
    {
        try
        {
            // Legacy-Felder nicht speichern
            _config.Tiles = null;
            _config.GridColumns = null;
            _config.GridRows = null;

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
        // Aus allen Dashboards entfernen
        foreach (var dashboard in _config.Dashboards)
        {
            dashboard.Tiles.RemoveAll(t => t.DataPointId == id);
        }
        SaveConfiguration();
    }

    public Dashboard? GetDashboard(string id)
    {
        return _config.Dashboards.FirstOrDefault(d => d.Id == id);
    }

    public void AddDashboard(Dashboard dashboard)
    {
        _config.Dashboards.Add(dashboard);
        SaveConfiguration();
    }

    public void UpdateDashboard(Dashboard dashboard)
    {
        var index = _config.Dashboards.FindIndex(d => d.Id == dashboard.Id);
        if (index >= 0)
        {
            _config.Dashboards[index] = dashboard;
            SaveConfiguration();
        }
    }

    public void DeleteDashboard(string id)
    {
        // Mindestens ein Dashboard muss bleiben
        if (_config.Dashboards.Count > 1)
        {
            _config.Dashboards.RemoveAll(d => d.Id == id);
            SaveConfiguration();
        }
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
            Dashboards = new List<Dashboard>
            {
                new()
                {
                    Id = "default",
                    Name = "Haupt-Dashboard",
                    GridColumns = 4,
                    GridRows = 3,
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
                            BackgroundColor = "#2196F3",
                            TextColor = "#FFFFFF"
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
                            BackgroundColor = "#4CAF50",
                            TextColor = "#FFFFFF"
                        }
                    }
                }
            }
        };
    }
}
