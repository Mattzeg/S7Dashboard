using S7.Net;
using S7Dashboard.Models;

namespace S7Dashboard.Services;

public class S7CommunicationService : IDisposable
{
    private Plc? _plc;
    private readonly ILogger<S7CommunicationService> _logger;
    private bool _isConnected;
    private readonly object _lockObject = new();

    public bool IsConnected => _isConnected;
    public string? LastError { get; private set; }

    public S7CommunicationService(ILogger<S7CommunicationService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> ConnectAsync(PlcConfiguration config)
    {
        try
        {
            lock (_lockObject)
            {
                _plc?.Close();
                _plc = new Plc(CpuType.S71500, config.IpAddress, (short)config.Rack, (short)config.Slot);
            }

            await Task.Run(() => _plc.Open());
            _isConnected = _plc.IsConnected;

            if (_isConnected)
            {
                LastError = null;
                _logger.LogInformation("Verbunden mit SPS {IpAddress}", config.IpAddress);
            }
            else
            {
                LastError = "Verbindung konnte nicht hergestellt werden";
                _logger.LogWarning("Verbindung zu SPS {IpAddress} fehlgeschlagen", config.IpAddress);
            }

            return _isConnected;
        }
        catch (Exception ex)
        {
            _isConnected = false;
            LastError = ex.Message;
            _logger.LogError(ex, "Fehler beim Verbinden mit SPS");
            return false;
        }
    }

    public void Disconnect()
    {
        lock (_lockObject)
        {
            _plc?.Close();
            _isConnected = false;
            _logger.LogInformation("SPS-Verbindung getrennt");
        }
    }

    public async Task<double?> ReadValueAsync(DataPointConfig dataPoint)
    {
        if (!_isConnected || _plc == null)
            return null;

        try
        {
            object? rawValue = null;

            await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    if (_plc == null || !_plc.IsConnected)
                        return;

                    rawValue = dataPoint.DataType.ToLower() switch
                    {
                        "real" => _plc.Read(DataType.DataBlock, dataPoint.DbNumber, dataPoint.StartByte, VarType.Real, 1),
                        "int" => _plc.Read(DataType.DataBlock, dataPoint.DbNumber, dataPoint.StartByte, VarType.Int, 1),
                        "dint" => _plc.Read(DataType.DataBlock, dataPoint.DbNumber, dataPoint.StartByte, VarType.DInt, 1),
                        "bool" => _plc.Read(DataType.DataBlock, dataPoint.DbNumber, dataPoint.StartByte, VarType.Bit, 1, 0),
                        _ => null
                    };
                }
            });

            if (rawValue == null)
                return null;

            double numericValue = rawValue switch
            {
                float f => f,
                double d => d,
                int i => i,
                short s => s,
                bool b => b ? 1.0 : 0.0,
                _ => Convert.ToDouble(rawValue)
            };

            // Skalierung anwenden
            return (numericValue * dataPoint.ScaleFactor) + dataPoint.Offset;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Lesen von Datenpunkt {Name}", dataPoint.Name);
            _isConnected = false;
            LastError = ex.Message;
            return null;
        }
    }

    public async Task<Dictionary<string, double?>> ReadAllValuesAsync(IEnumerable<DataPointConfig> dataPoints)
    {
        var results = new Dictionary<string, double?>();

        foreach (var dataPoint in dataPoints)
        {
            results[dataPoint.Id] = await ReadValueAsync(dataPoint);
        }

        return results;
    }

    public async Task<bool> TestConnectionAsync(PlcConfiguration config)
    {
        try
        {
            using var testPlc = new Plc(CpuType.S71500, config.IpAddress, (short)config.Rack, (short)config.Slot);
            await Task.Run(() => testPlc.Open());
            var isConnected = testPlc.IsConnected;
            testPlc.Close();
            return isConnected;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Verbindungstest fehlgeschlagen");
            LastError = ex.Message;
            return false;
        }
    }

    public void Dispose()
    {
        _plc?.Close();
        _plc = null;
    }
}
