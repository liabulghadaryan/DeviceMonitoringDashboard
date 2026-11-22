namespace DeviceMonitoringDashboard;

public class MonitoringSettings
{
    public List<DeviceEndpoint> Devices { get; set; } = [];
}

public class DeviceEndpoint
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class DeviceInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Dictionary<string, string> Metrics { get; set; } = new();
}

public record DeviceStatus(
    string Name,
    string Url,
    bool Online,
    string? Address,
    long? UptimeSeconds,
    double? TemperatureC,
    int? ResponseMs,
    string? Error);

public record MonitoringSnapshot(
    int Total,
    int Online,
    int Offline,
    DateTimeOffset CheckedAt,
    IReadOnlyList<DeviceStatus> Devices);
