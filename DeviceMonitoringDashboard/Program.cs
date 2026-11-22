using System.Net.Security;
using DeviceMonitoringDashboard;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MonitoringSettings>(builder.Configuration.GetSection("DeviceMonitoringDashboard"));
builder.Services.AddSingleton<DeviceMonitor>();

builder.Services.AddHttpClient("devices", client => client.Timeout = TimeSpan.FromSeconds(2))
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (request, _, _, errors) =>
            errors == SslPolicyErrors.None || (request.RequestUri?.IsLoopback ?? false)
    });

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/devices", (DeviceMonitor monitor, CancellationToken token) => monitor.CheckAllAsync(token));

app.Run();
