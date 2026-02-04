namespace S7Dashboard.Models;

public class TileConfig
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string DataPointId { get; set; } = string.Empty;
    public int PositionX { get; set; } = 1;
    public int PositionY { get; set; } = 1;
    public int Width { get; set; } = 1;
    public int Height { get; set; } = 1;
    public int TitleFontSize { get; set; } = 16;
    public int ValueFontSize { get; set; } = 32;
    public string BackgroundColor { get; set; } = "#2196F3";
    public string TextColor { get; set; } = "#FFFFFF";
}
