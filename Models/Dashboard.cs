namespace S7Dashboard.Models;

public class Dashboard
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "Neues Dashboard";
    public int GridColumns { get; set; } = 4;
    public int GridRows { get; set; } = 3;
    public List<TileConfig> Tiles { get; set; } = new();
}
