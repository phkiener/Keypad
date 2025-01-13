namespace PlainDeck.Sdk.Hosting;

public sealed partial class DeviceHost : IDeviceContext
{
    public DeviceConfiguration Device => context.Device;
    public void SetBrightness(double percentage) => context.SetBrightness(percentage);
    public void SetKeyImage(DeviceKey key, byte[] imageData) => context.SetKeyImage(key, imageData);
}
