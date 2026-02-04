namespace S7Dashboard.Models;

public class PlcConfiguration
{
    public string IpAddress { get; set; } = "192.168.0.1";
    public int Rack { get; set; } = 0;
    public int Slot { get; set; } = 1;
    public int PollingIntervalMs { get; set; } = 1000;
}
