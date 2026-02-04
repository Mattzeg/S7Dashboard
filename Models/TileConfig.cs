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

    // Grenzwert-basierte Farbgebung
    public bool UseThresholdColors { get; set; } = false;
    public double? LowerThreshold { get; set; }
    public double? UpperThreshold { get; set; }
    public string ColorBelowLower { get; set; } = "#2196F3";  // Blau (unter unterer Grenze)
    public string ColorInRange { get; set; } = "#4CAF50";     // Grün (im Bereich)
    public string ColorAboveUpper { get; set; } = "#F44336"; // Rot (über oberer Grenze)
}
