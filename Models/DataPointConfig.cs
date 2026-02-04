namespace S7Dashboard.Models;

public class DataPointConfig
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public int DbNumber { get; set; }
    public int StartByte { get; set; }
    public string DataType { get; set; } = "Real"; // "Real", "Int", "DInt", "Bool"
    public string Unit { get; set; } = string.Empty;
    public double ScaleFactor { get; set; } = 1.0;
    public double Offset { get; set; } = 0.0;
    public int DecimalPlaces { get; set; } = 2;
}
