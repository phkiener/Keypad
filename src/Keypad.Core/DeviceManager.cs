using HidSharp;
using Keypad.Core.Abstractions;
using Keypad.Core.Device;

namespace Keypad.Core;

/// <summary>
/// Helper to connect to StreamDeck devices
/// </summary>
public static class DeviceManager
{
    private const int vendorId = 0x0FD9;

    /// <summary>
    /// Try to connect to a specific device
    /// </summary>
    /// <param name="deviceType">The type of device to connect to</param>
    /// <param name="serialNumber">The serial number of the specific device; if <c>null</c>, the first matching device is connected to</param>
    /// <returns>A <see cref="ConnectedDevice"/> if a connection could be established or <c>null</c> if no such device was found</returns>
    public static ConnectedDevice? Connect(DeviceType? deviceType, string? serialNumber = null)
    {
        var device = DeviceList.Local.GetHidDeviceOrNull(vendorId, (int?)deviceType, serialNumber: serialNumber);
        if (device is null)
        {
            return null;
        }

        return device.ProductID switch
        {
            (int)DeviceType.StreamDeckXL2022 => new StreamDeckXL2022(device),
            (int)DeviceType.StreamDeckPedal => new StreamDeckPedal(device),
            _ => null
        };
    }

    /// <summary>
    /// Enumerate the serial numbers of all supported devices
    /// </summary>
    /// <returns>An enumerable over all devices that may be connected to using <see cref="Connect"/></returns>
    public static IEnumerable<string> EnumerateDevices()
    {
        foreach (var device in DeviceList.Local.GetHidDevices(vendorId))
        {
            if (Enum.GetValues<DeviceType>().Any(e => device.ProductID == (int)e))
            {
                yield return device.GetSerialNumber();
            }
        }
    }

    /// <summary>
    /// Checks if the given device is connected
    /// </summary>
    /// <param name="device">The device whose connection to check</param>
    /// <returns><c>true</c> if the device is still connected, <c>false</c> otherwise</returns>
    public static bool IsConnected(ConnectedDevice device)
    {
        return DeviceList.Local.GetHidDeviceOrNull(vendorId, (int)device.DeviceType, serialNumber: device.SerialNumber) is not null;
    }
}
