using Keypad.Configuration;
using Keypad.Core;
using Keypad.Core.Abstractions;

namespace Keypad;

/// <summary>
/// A hub to manage multiple <see cref="ConnectedDevice"/>s based on a <see cref="KeypadConfig"/>
/// </summary>
public sealed class DeviceHub : IDisposable
{
    private readonly KeypadConfig config;
    private readonly IKeyEmitter keyEmitter;
    private readonly List<ConnectedDevice> connectedDevices = [];
    private readonly Timer devicePollingTimer;
    
    /// <summary>
    /// Create a new hub and poll for new devices
    /// </summary>
    /// <param name="config">The configuration to use for devices</param>
    /// <param name="keyEmitter">The <see cref="IKeyEmitter"/> to use for emulating key pressed</param>
    public DeviceHub(KeypadConfig config, IKeyEmitter keyEmitter)
    {
        this.config = config;
        this.keyEmitter = keyEmitter;

        devicePollingTimer = new Timer(
            callback: _ => OnDevicesChanged(),
            state: null,
            dueTime: TimeSpan.Zero,
            period: TimeSpan.FromSeconds(5));
    }

    /// <summary>
    /// <c>true</c> if any device is connected
    /// </summary>
    public bool IsConnected => ConnectedDevices.Any();
    
    /// <summary>
    /// The names of all connected devices, if any
    /// </summary>
    public IReadOnlyList<string> ConnectedDevices => connectedDevices.Select(d => $"{d.DeviceType}:{d.SerialNumber}").ToList();

    /// <summary>
    /// A callback that is invoked every time a connection is added or removed
    /// </summary>
    public EventHandler? OnConnectionsChanged;

    private void OnDevicesChanged()
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

    private EmulatedKey? GetConfiguredKey(object? sender, DeviceButton button)
    {
        if (sender is not ConnectedDevice device)
        {
            return null;
        }

        var deviceConfig = config.Devices.SingleOrDefault(d => Matches(device, d));
        var keyConfiguration = deviceConfig?.Keys.SingleOrDefault(k => k.Button == button);
        
        return keyConfiguration?.Key;
    }

    private void OnKeyPressed(object? sender, DeviceButton e)
    {
        var key = GetConfiguredKey(sender, e);
        if (key.HasValue)
        {
            
            keyEmitter.Press(key.Value);
        }
    }

    private void OnKeyReleased(object? sender, DeviceButton e)
    {
        var key = GetConfiguredKey(sender, e);
        if (key.HasValue)
        {
            
            keyEmitter.Release(key.Value);
        }
    }

    /// <inheritdoc />
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
