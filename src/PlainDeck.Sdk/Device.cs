using PlainDeck.Sdk.Hosting;

namespace PlainDeck.Sdk;

public static class Device
{
    private const int VendorId = 0x0FD9;

    public static DeviceHost Connect(DeviceType type)
    {
        var possibleDevices = HidSharp.DeviceList.Local.GetHidDevices(vendorID: VendorId, productID: (int)type);
        var chosenDevice = possibleDevices.FirstOrDefault() ?? throw new InvalidOperationException($"No {type} found.");
        
        return new DeviceHost(chosenDevice, DeviceConfiguration.Resolve(type));
    }
}
