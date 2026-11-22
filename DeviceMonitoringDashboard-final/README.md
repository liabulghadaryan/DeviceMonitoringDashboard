# DeviceMonitoringDashboard

A web dashboard that shows whether each device is online or offline, with its address,
uptime, temperature and reply time. The top of the page shows how many devices are online.

The repository has two projects:

- `Device.csproj` (repository root): the device from the workshop, which answers `GET /info`
- `DeviceMonitoringDashboard/`: the dashboard, which asks every configured device for `/info`

## Build

```cmd
dotnet build
```

## Run

Open three terminals in the repository folder.

```cmd
dotnet run --no-build --launch-profile Device1
```

```cmd
dotnet run --no-build --launch-profile Device2
```

```cmd
cd DeviceMonitoringDashboard
dotnet run
```

Open http://localhost:5050. You should see "2 of 2 devices online".
Press Ctrl+C in the Device2 terminal and the page shows "1 of 2 devices online".

To add a device, add its address to `DeviceMonitoringDashboard/appsettings.json`.
