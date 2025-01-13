using PlainDeck.Sdk.Hosting;

namespace PlainDeck.Sdk;

public interface IDeviceContext
{
    DeviceConfiguration Device { get; }
    void SetBrightness(double percentage);
    void SetKeyImage(DeviceKey key, byte[] imageData);
}