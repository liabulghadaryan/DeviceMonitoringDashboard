using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace DeviceMonitoringDashboard;

public class DeviceMonitor(IHttpClientFactory httpClientFactory, IOptions<MonitoringSettings> options)
{
    public async Task<MonitoringSnapshot> CheckAllAsync(CancellationToken token)
    {
        var devices = options.Value.Devices;
        var statuses = await Task.WhenAll(devices.Select(d => CheckAsync(d, token)));
        var online = statuses.Count(s => s.Online);

        return new MonitoringSnapshot(statuses.Length, online, statuses.Length - online, DateTimeOffset.UtcNow, statuses);
    }

    private async Task<DeviceStatus> CheckAsync(DeviceEndpoint device, CancellationToken token)
    {
        var timer = Stopwatch.StartNew();
        try
        {
            var client = httpClientFactory.CreateClient("devices");
            var infoUri = new Uri(new Uri(device.Url.TrimEnd('/') + "/"), "info");
            var info = await client.GetFromJsonAsync<DeviceInfoDto>(infoUri, token)
                       ?? throw new JsonException("Empty response");

            var metrics = new Dictionary<string, string>(info.Metrics, StringComparer.OrdinalIgnoreCase);

            return new DeviceStatus(
                string.IsNullOrWhiteSpace(info.Name) ? device.Name : info.Name,
                device.Url,
                Online: true,
                info.Address,
                metrics.TryGetValue("UptimeSeconds", out var up) && long.TryParse(up, CultureInfo.InvariantCulture, out var uptime) ? uptime : null,
                metrics.TryGetValue("Temperature", out var t) && double.TryParse(t, CultureInfo.InvariantCulture, out var temp) ? temp : null,
                (int)timer.ElapsedMilliseconds,
                Error: null);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or UriFormatException)
        {
            if (token.IsCancellationRequested)
            {
                throw;
            }

            var reason = ex switch
            {
                TaskCanceledException => "No answer within 2 seconds",
                HttpRequestException => "Cannot connect to the device",
                _ => "Unexpected reply from the device"
            };

            return new DeviceStatus(device.Name, device.Url, false, null, null, null, null, reason);
        }
    }
}
