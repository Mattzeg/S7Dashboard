namespace S7Dashboard.Models;

public class DashboardConfig
{
    public PlcConfiguration PlcConfiguration { get; set; } = new();
    public List<DataPointConfig> DataPoints { get; set; } = new();
    public List<TileConfig> Tiles { get; set; } = new();
    public int GridColumns { get; set; } = 4;
    public int GridRows { get; set; } = 3;
}
