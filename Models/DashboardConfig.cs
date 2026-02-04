namespace S7Dashboard.Models;

public class DashboardConfig
{
    public PlcConfiguration PlcConfiguration { get; set; } = new();
    public List<DataPointConfig> DataPoints { get; set; } = new();
    public List<Dashboard> Dashboards { get; set; } = new();
    public string? SettingsPin { get; set; }

    // Legacy-Felder für Abwärtskompatibilität
    public List<TileConfig>? Tiles { get; set; }
    public int? GridColumns { get; set; }
    public int? GridRows { get; set; }
}
