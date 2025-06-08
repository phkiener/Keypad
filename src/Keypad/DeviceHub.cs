using Keypad.Configuration;
using Keypad.Core;
using Keypad.Core.Abstractions;

namespace Keypad;

public sealed class DeviceHub : IDisposable
{
    private readonly KeypadConfig config;
    private readonly List<ConnectedDevice> connectedDevices = [];
    private readonly Timer devicePollingTimer;
    
    public DeviceHub(KeypadConfig config)
    {
        this.config = config;
        devicePollingTimer = new Timer(_ => OnDevicesChanged(this, EventArgs.Empty), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
    }

    public bool IsConnected => ConnectedDevices.Any();
    public IReadOnlyList<string> ConnectedDevices => connectedDevices.Select(d => $"{d.DeviceType}:{d.SerialNumber}").ToList();

    public EventHandler? OnConnectionsChanged;

    private void OnDevicesChanged(object? sender, EventArgs empty)
    {
        lock (connectedDevices)
        {
            var disconnectedDevices = connectedDevices.Where(static d => !DeviceManager.IsConnected(d)).ToList();
            foreach (var disconnectedDevice in disconnectedDevices)
            {
                connectedDevices.Remove(disconnectedDevice);
                disconnectedDevice.Dispose();

                OnConnectionsChanged?.Invoke(this, EventArgs.Empty);
            }

            foreach (var targetDevice in config.Devices)
            {
                if (connectedDevices.Any(d => Matches(d, targetDevice)))
                {
                    continue;
                }

                var device = DeviceManager.Connect(targetDevice.Type, targetDevice.SerialNumber);
                if (device is not null)
                {
                    connectedDevices.Add(device);
                    OnConnectionsChanged?.Invoke(this, EventArgs.Empty);

                    device.SetBrightness(targetDevice.Brightness);
                    foreach (var button in device.Buttons)
                    {
                        var image = targetDevice.Keys.SingleOrDefault(k => k.Button == button)?.Image
                                    ?? new DeviceImage.Color("black");

                        device.SetImage(button, image);
                    }

                    device.KeyPressed += OnKeyPressed;
                    device.KeyReleased += OnKeyReleased;
                }
            }
        }
    }

    private void OnKeyPressed(object? sender, DeviceButton e)
    {
        if (sender is not ConnectedDevice device)
        {
            return;
        }

        var deviceConfig = config.Devices.SingleOrDefault(d => Matches(device, d));
        if (deviceConfig is null)
        {
            return;
        }
        
        Console.WriteLine($"{device.DeviceType}:{device.SerialNumber} pressed {e}");
    }

    private void OnKeyReleased(object? sender, DeviceButton e)
    {
        if (sender is not ConnectedDevice device)
        {
            return;
        }

        var deviceConfig = config.Devices.SingleOrDefault(d => Matches(device, d));
        if (deviceConfig is null)
        {
            return;
        }

        Console.WriteLine($"{device.DeviceType}:{device.SerialNumber} released {e}");
    }

    public void Dispose()
    {
        devicePollingTimer.Dispose();

        foreach (var device in connectedDevices)
        {
            device.KeyPressed -= OnKeyPressed;
            device.KeyReleased -= OnKeyReleased;
            device.Dispose();
        }
    }

    private static bool Matches(ConnectedDevice device, KeypadDeviceConfiguration deviceConfiguration)
    {
        return deviceConfiguration.Type == device.DeviceType
               && (deviceConfiguration.SerialNumber is null || deviceConfiguration.SerialNumber == device.SerialNumber);
    }
}
