using System.Runtime.CompilerServices;
using HidSharp;
using Keypad.Core.Device;

namespace Keypad.Core;

/// <summary>
/// Helper to connect to StreamDeck devices
/// </summary>
public static class DeviceManger
{
    private const int vendorId = 0x0FD9;

    /// <summary>
    /// Try to connect to a specific device
    /// </summary>
    /// <param name="deviceType">The type of device to connect to</param>
    /// <param name="serialNumber">The serial number of the specific device; if <c>null</c>, the first matching device is connected to</param>
    /// <returns>A <see cref="ConnectedDevice"/> if a connection could be established or <c>null</c> if no such device was found</returns>
    public static ConnectedDevice? Connect(DeviceType deviceType, string? serialNumber = null)
    {
        var device = DeviceList.Local.GetHidDeviceOrNull(vendorId, (int)deviceType, serialNumber: serialNumber);
        if (device is null)
        {
            return null;
        }

        return deviceType switch
        {
            DeviceType.StreamDeckXL2022 => new StreamDeckXL2022(device),
            _ => null
        };
    }

#pragma warning disable CA2255 // Provide a "wrapper" for DeviceList.Local.Changed so that consumers don't need to work with HidSharp directly
    [ModuleInitializer]
    internal static void ConnectCallback()
    {
        DeviceList.Local.Changed += (sender, _) => AvailableDevicesChanged?.Invoke(sender, EventArgs.Empty);
    }
#pragma warning restore CA2255
    
    /// <summary>
    /// Invoked whenever a USB-device connects or disconnects
    /// </summary>
    public static event EventHandler? AvailableDevicesChanged; 
}
